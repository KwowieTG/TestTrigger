
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kaseya.AuthAnvil.Models
{
    [DebuggerDisplay("{Type} {State} {ChangedProperties.Count}")]
    public class EntityChange : IEquatable<EntityChange>
    {
        public Guid TenantId { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid EntityId { get; set; }

        public ChangeState State { get; set; }

        public string Type { get; set; }

        public Guid ProvisioningPolicyId { get; set; }

        private Dictionary<string, IEnumerable<string>> changes;

        public Dictionary<string, IEnumerable<string>> ChangedProperties
        {
            get { return changes ?? (changes = new Dictionary<string, IEnumerable<string>>()); }
            set { changes = value; }
        }

        public long Timestamp { get; set; }

        public override int GetHashCode()
        {
            return TenantId.GetHashCode() ^
                   OrganizationId.GetHashCode() ^
                   EntityId.GetHashCode() ^
                   State.GetHashCode() ^
                   ChangedProperties.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as EntityChange;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(EntityChange other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.GetHashCode() == GetHashCode();
        }

        private static readonly EntityChangeComparer comparer = new EntityChangeComparer();

        public static IEqualityComparer<EntityChange> EqualityComparer { get { return comparer; } }

        private class EntityChangeComparer : IEqualityComparer<EntityChange>
        {
            public bool Equals(EntityChange x, EntityChange y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.Equals(y);
            }

            public int GetHashCode(EntityChange obj)
            {
                return obj.GetHashCode();
            }
        }
    }
    public enum ChangeState
    {
        Detached = 1,
        Unchanged = 2,
        Added = 4,
        Deleted = 8,
        Modified = 16,
        Removed = 32
    }
}

