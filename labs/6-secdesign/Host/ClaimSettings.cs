namespace SecureByDesign.Host
{
    public static class ClaimSettings
    {
        //Authorization server scopes
        public const string ProductsRead = "products.read";
        public const string ProductsWrite = "products.write";

        //Local permission claim types
        public const string UrnLocalProductRead = "urn:local:product:read";
        public const string UrnLocalProductWrite = "urn:local:product:write";
        public const string UrnLocalProductIds = "urn:local:product:ids";  
    }
}

