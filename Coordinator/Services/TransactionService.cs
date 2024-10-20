using Coordinator.Context;
using Coordinator.Enums;
using Coordinator.Models;
using Coordinator.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services;

public class TransactionService(IHttpClientFactory _httpFactory, TwoPhaseCommitContext _context) : ITransactionService
{
    private readonly HttpClient _orderHttpClient = _httpFactory.CreateClient("Order.Api");
    private readonly HttpClient _stockHttpClient = _httpFactory.CreateClient("Stock.Api");
    private readonly HttpClient _paymentHttpClient = _httpFactory.CreateClient("Payment.Api");

    public async Task<Guid> CreateTransactionAsync()
    {
        Guid transactionId = Guid.NewGuid();

        var nodes = await _context.Nodes.ToListAsync();

        nodes.ForEach(node => node.NodeStates = new List<NodeState>()
            {
                new NodeState(transactionId)
                    { IsReady = ReadyType.Pending, TransactionState = TransactionState.Pending }
            }
        );

        await _context.SaveChangesAsync();

        return transactionId;
    }

    public async Task PrepareServicesAsync(Guid transactionId)
    {
        _httpFactory.CreateClient("OrderApi");

        var transactionNodes = await _context.NodeStates
            .Include(ns => ns.Node)
            .Where(ns => ns.TransactionId == transactionId)
            .ToListAsync();

        foreach (var transactionNode in transactionNodes)
        {
            try
            {
                var response = await (transactionNode.Node.Name switch
                {
                    "Order.Api" => _orderHttpClient.GetAsync("ready"),
                    "Payment.Api" => _paymentHttpClient.GetAsync("ready"),
                    "Stock.Api" => _stockHttpClient.GetAsync("ready"),
                });

                var result = bool.Parse(await response.Content.ReadAsStringAsync());

                transactionNode.IsReady = result ? ReadyType.Ready : ReadyType.Unready;
            }
            catch (Exception e)
            {
                transactionNode.IsReady = ReadyType.Unready;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
    {
        var transactionNodes = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .ToListAsync();

        return transactionNodes.TrueForAll(ns => ns.IsReady == ReadyType.Ready);
    }

    public async Task CommitAsync(Guid transactionId)
    {
        var transactionNodes = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .Include(ns => ns.Node)
            .ToListAsync();

        foreach (var transactionNode in transactionNodes)
        {
            try
            {
                var response = await (transactionNode.Node.Name switch
                {
                    "Order.Api" => _orderHttpClient.GetAsync("commit"),
                    "Payment.Api" => _paymentHttpClient.GetAsync("commit"),
                    "Stock.Api" => _stockHttpClient.GetAsync("commit"),
                });

                var result = bool.Parse(await response.Content.ReadAsStringAsync());

                transactionNode.TransactionState = result ? TransactionState.Done : TransactionState.Abort;
            }
            catch (Exception e)
            {
                transactionNode.TransactionState = TransactionState.Abort;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
    {
        var transactionNodes = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .ToListAsync();

        return transactionNodes.TrueForAll(ns => ns.TransactionState == TransactionState.Done);
    }

    public async Task RollbackAsync(Guid transactionId)
    {
        var transactionNodes = await _context.NodeStates
            .Where(ns => ns.TransactionId == transactionId)
            .Include(ns => ns.Node)
            .ToListAsync();

        foreach (var transactionNode in transactionNodes)
        {
            try
            {
                if (transactionNode.TransactionState == Enums.TransactionState.Done)
                {
                    _ = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("rollback"),
                        "Stock.API" => _stockHttpClient.GetAsync("rollback"),
                        "Payment.API" => _paymentHttpClient.GetAsync("rollback"),
                    });
                }

                transactionNode.TransactionState = TransactionState.Abort;
            }
            catch (Exception e)
            {
                transactionNode.TransactionState = TransactionState.Abort;
            }
        }

        await _context.SaveChangesAsync();
    }
}