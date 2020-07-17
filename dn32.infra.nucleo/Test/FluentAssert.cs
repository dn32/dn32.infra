// -----------------------------------------------------------------------
// <copyright company="DnControlador System">
//     Copyright © DnControlador System. All rights reserved.
//     TODOS OS DIREITOS RESERVADOS.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using dn32.infra;

namespace dn32.infra {
    public static class DnAssert {
        public static void Equal (object obj1, object obj2) {
            if (!obj1.CompareObjects (obj2)) {
                throw new ValidationException ("The objects are different");
            }
        }

        public static void IsNotNullOrEmpty (object entity) {
            if (entity.IsDnNull ()) {
                throw new ValidationException ("The text is empty, null or space");
            }
        }
    }
}