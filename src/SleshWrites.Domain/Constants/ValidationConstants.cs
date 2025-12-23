namespace SleshWrites.Domain.Constants;

/// <summary>
/// Centralized validation constants for entity field lengths and constraints.
/// Used by validators, EF configurations, and domain logic.
/// </summary>
public static class ValidationConstants
{
    public static class BlogPost
    {
        public const int TitleMaxLength = 200;
        public const int ContentMinLength = 50;
        public const int ExcerptMaxLength = 500;
        public const int FeaturedImageMaxLength = 2048;
        public const int StatusMaxLength = 20;
    }

    public static class Category
    {
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 500;
    }

    public static class Tag
    {
        public const int NameMaxLength = 50;
    }

    public static class Author
    {
        public const int DisplayNameMaxLength = 100;
        public const int EmailMaxLength = 256;
        public const int AzureAdB2CIdMaxLength = 36;
        public const int BioMaxLength = 1000;
        public const int AvatarUrlMaxLength = 2048;
    }

    public static class Slug
    {
        public const int MaxLength = 200;
    }

    public static class MetaData
    {
        public const int TitleMaxLength = 60;
        public const int DescriptionMaxLength = 160;
        public const int KeywordsMaxLength = 200;
        public const int CanonicalUrlMaxLength = 2048;
        public const int OgImageMaxLength = 2048;
    }

    public static class SocialLink
    {
        public const int PlatformMaxLength = 50;
        public const int UrlMaxLength = 2048;
    }
}
