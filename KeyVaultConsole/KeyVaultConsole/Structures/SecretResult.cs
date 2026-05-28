namespace KeyVaultConsole.Structures
{
    internal class SecretResult
    {
        internal string Name { get; set; } = "";
        internal string Value { get; set; } = "";
        internal string Version { get; set; } = "";
        internal string ContentType { get; set; } = "";
        internal string CreatedOn { get; set; } = "";
        internal object? Tags { get; set; }
        internal string Status { get; set; } = "";
    }
}
