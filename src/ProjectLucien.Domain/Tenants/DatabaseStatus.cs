namespace ProjectLucien.Domain.Tenants;

public enum DatabaseStatus
{
    Unknown = 0,
    Pending = 1,
    Provisioning = 2,
    Ready = 3,
    Error = 4,
    Decommissioned = 5
}