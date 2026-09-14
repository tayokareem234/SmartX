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
                null,
                errors);

            return errors;
        }

        private void ValidateNodeRecursive(
            DeploymentNode node,
            string parentPath,
            string? parentType,
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
            else
            {
                ValidateNodeType(
                    node.NodeType,
                    currentPath,
                    errors);

                ValidateParentChildRelationship(
                    parentType,
                    node.NodeType,
                    currentPath,
                    errors);
            }

            foreach (DeploymentNode child
                     in node.Children)
            {
                ValidateNodeRecursive(
                    child,
                    currentPath,
                    node.NodeType,
                    errors);
            }
        }

        private void ValidateNodeType(
            string nodeType,
            string currentPath,
            List<string> errors)
        {
            string[] allowedTypes =
            {
                "Facility",
                "Zone",
                "Sub-Zone",
                "Room",
                "Sensor"
            };

            bool validType =
                allowedTypes.Any(type =>
                    type.Equals(
                        nodeType,
                        StringComparison.OrdinalIgnoreCase));

            if (!validType)
            {
                errors.Add(
                    $"{currentPath}: Unsupported node type '{nodeType}'. " +
                    "Allowed types are Facility, Zone, Sub-Zone, Room and Sensor.");
            }
        }

        private void ValidateParentChildRelationship(
            string? parentType,
            string childType,
            string currentPath,
            List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(parentType))
            {
                return;
            }

            bool validRelationship =
                parentType.Equals(
                    "Facility",
                    StringComparison.OrdinalIgnoreCase)
                    ? childType.Equals(
                        "Zone",
                        StringComparison.OrdinalIgnoreCase)

                : parentType.Equals(
                    "Zone",
                    StringComparison.OrdinalIgnoreCase)
                    ? childType.Equals(
                        "Sub-Zone",
                        StringComparison.OrdinalIgnoreCase)
                      || childType.Equals(
                        "Room",
                        StringComparison.OrdinalIgnoreCase)

                : parentType.Equals(
                    "Sub-Zone",
                    StringComparison.OrdinalIgnoreCase)
                    ? childType.Equals(
                        "Room",
                        StringComparison.OrdinalIgnoreCase)
                      || childType.Equals(
                        "Sensor",
                        StringComparison.OrdinalIgnoreCase)

                : parentType.Equals(
                    "Room",
                    StringComparison.OrdinalIgnoreCase)
                    ? childType.Equals(
                        "Sensor",
                        StringComparison.OrdinalIgnoreCase)

                : parentType.Equals(
                    "Sensor",
                    StringComparison.OrdinalIgnoreCase)
                    ? false

                : true;

            if (!validRelationship)
            {
                errors.Add(
                    $"{currentPath}: Invalid deployment hierarchy. " +
                    $"A {childType} cannot be placed directly under a {parentType}.");
            }
        }
    }
}