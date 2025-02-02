using Coordinator.Enums;

namespace Coordinator.Models;

public record NodeState(Guid TransactionId)
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// 1. aşamanın durumunu ifade eder
    /// </summary>
    public ReadyType IsReady { get; set; }
    
    /// <summary>
    /// 2. aşama neticesinde işlemin başarıyla tamamlanıp, tammalanmadığını ifade eder
    /// </summary>
    public TransactionState TransactionState { get; set; }
    public Node Node { get; set; }
}