using System.Reflection;

namespace Madia.FarmShop.Patches
{
    internal static class PatchUtils
    {
        public static void SetPrivateValue(object instance, string name, object newValue)
        {
            instance.GetType()
                .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(instance, newValue);
        }

        public static T GetPrivateValue<T>(object instance, string name)
        {
            return (T)instance.GetType()
                .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(instance);
        }

        public static T GetPrivateStaticValue<C, T>(string name)
        {
            return (T)typeof(C)
                .GetField(name, BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null);
        }
    }
}
