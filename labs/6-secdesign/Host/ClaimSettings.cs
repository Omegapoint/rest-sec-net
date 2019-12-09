namespace SecureByDesign.Host
{
    public static class ClaimSettings
    {
        //Token attributes
        public const string Scope = "scope";

        //Local claim types
        public const string UrnLocalUsername = "urn:local:username";
        public const string UrnLocalProductIds = "urn:local:product:ids";

        //Permissions
        public const string ProductsRead = "products.read";
        public const string ProductsWrite = "products.write";
    }

}

