namespace SchoolWebRegister.Domain.Permissions
{
    [Flags]
    public enum Permissions : byte
    {
        Read = 0x00,
        Create = 0x01,
        Edit = 0x02,
        Delete = 0x04,
        Admin = (Read | Create | Edit | Delete)
    }

    public static class PermissionsExtensions
    {
        public static string Serialize(this Permissions permissions)
        {
            return string.Concat(permissions.GetType().Namespace, ".", permissions.ToString());
        }
    }
}
