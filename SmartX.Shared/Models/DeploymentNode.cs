namespace SmartX.Shared.Models
{
    public class DeploymentNode
    {
        public string NodeId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string NodeType { get; set; } = string.Empty;

        public List<DeploymentNode> Children { get; set; } = new();
    }
}