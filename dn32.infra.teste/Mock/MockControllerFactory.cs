// -----------------------------------------------------------------------
// <copyright company="DnControlador System">
//     Copyright © DnControlador System. All rights reserved.
//     TODOS OS DIREITOS RESERVADOS.
// </copyright>
// -----------------------------------------------------------------------

using System;


namespace dn32.infra.Mock
{
    public static class MockControllerFactory
    {
        public static TC Create<TC>() where TC : DnControllerBase, new()
        {
            return Create(typeof(TC)) as TC;
        }

        public static DnControllerBase Create(Type controllerType)
        {
            return Activator.CreateInstance(controllerType) as DnControllerBase;
        }
    }
}