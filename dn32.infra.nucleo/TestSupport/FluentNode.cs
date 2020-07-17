using System;
using System.Collections.Generic;

namespace dn32.infra {
    public class DnNode {
        public Type EntityType { get; set; }
        public string EntityTypeName => EntityType.Name;
        public List<DnNode> ReferencePointers { get; set; } = new List<DnNode> ();
        public bool IsPrimitive { get; set; }
        public object Instance { get; set; }
    }
}