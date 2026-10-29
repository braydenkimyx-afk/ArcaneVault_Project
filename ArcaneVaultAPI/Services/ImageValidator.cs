// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

namespace ArcaneVaultAPI.Services
{
    public static class ImageValidator
    {
        public static (bool IsValid, string ErrorMessage) ValidateImage(byte[]? imageBytes, string? declaredContentType)
        {
            // Null is valid
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return (true, string.Empty);
            }

            // Minimum length - anything under 100 bytes is malformed
            if (imageBytes.Length < 100)
            {
                return (false, "Image file is malformed (too small).");
            }

            // Size cap: 2MB (2 * 1024 * 1024 = 2097152 bytes)
            const int maxBytes = 2 * 1024 * 1024;
            if (imageBytes.Length > maxBytes)
            {
                return (false, $"Image is too large. Actual size: {imageBytes.Length} bytes, Permitted size: {maxBytes} bytes.");
            }

            // Content type validation
            if (declaredContentType != "image/jpeg" && declaredContentType != "image/png")
            {
                return (false, "Only JPEG or PNG images are permitted.");
            }

            // Magic byte verification
            if (declaredContentType == "image/jpeg")
            {
                // JPEG: FF D8 FF
                if (imageBytes[0] != 0xFF || imageBytes[1] != 0xD8 || imageBytes[2] != 0xFF)
                {
                    return (false, "Invalid image format. Detected content does not match declared JPEG type.");
                }
            }
            else if (declaredContentType == "image/png")
            {
                // PNG: 89 50 4E 47 0D 0A 1A 0A
                if (imageBytes[0] != 0x89 || imageBytes[1] != 0x50 || imageBytes[2] != 0x4E || imageBytes[3] != 0x47 ||
                    imageBytes[4] != 0x0D || imageBytes[5] != 0x0A || imageBytes[6] != 0x1A || imageBytes[7] != 0x0A)
                {
                    return (false, "Invalid image format. Detected content does not match declared PNG type.");
                }
            }

            return (true, string.Empty);
        }
    }
}
