using SmartX.Shared.Models;

namespace SmartX.Api.Services
{
    public class DeploymentValidationService
    {
        public List<string> ValidateDeployment(
            DeploymentNode root)
        {
            var errors = new List<string>();

            ValidateNodeRecursive(
                root,
                "Facility",
                errors);

            return errors;
        }

        private void ValidateNodeRecursive(
            DeploymentNode node,
            string parentPath,
            List<string> errors)
        {
            if (node == null)
            {
                errors.Add(
                    $"{parentPath}: node is missing.");

                return;
            }

            string currentPath =
                $"{parentPath} > {node.Name}";

            if (string.IsNullOrWhiteSpace(
                node.NodeId))
            {
                errors.Add(
                    $"{currentPath}: NodeId is required.");
            }

            if (string.IsNullOrWhiteSpace(
                node.Name))
            {
                errors.Add(
                    $"{currentPath}: Name is required.");
            }

            if (string.IsNullOrWhiteSpace(
                node.NodeType))
            {
                errors.Add(
                    $"{currentPath}: NodeType is required.");
            }

            foreach (DeploymentNode child
                     in node.Children)
            {
                ValidateNodeRecursive(
                    child,
                    currentPath,
                    errors);
            }
        }
    }
}