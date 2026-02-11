namespace AdvancedDevSample.Domain.Enums
{
    /// <summary>
    /// État d'une commande
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// En attente
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Confirmée
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Annulée
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Complétée
        /// </summary>
        Completed = 4
    }
}