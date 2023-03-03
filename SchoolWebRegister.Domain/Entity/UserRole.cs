namespace SchoolWebRegister.Domain.Entity
{
    public enum UserRole : byte
    {
        Guest = 0x00,
        Student = 0x01,
        Teacher = 0x02,
        Administrator = 0x04
    }
}
