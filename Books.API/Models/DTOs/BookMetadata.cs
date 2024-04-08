using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Books.API.Models.DTOs
{
    public record AccessInfo(
     [property: JsonProperty("country")]
        [property: JsonPropertyName("country")] string country,
     [property: JsonProperty("viewability")]
        [property: JsonPropertyName("viewability")] string viewability,
     [property: JsonProperty("embeddable")]
        [property: JsonPropertyName("embeddable")] bool embeddable,
     [property: JsonProperty("publicDomain")]
        [property: JsonPropertyName("publicDomain")] bool publicDomain,
     [property: JsonProperty("textToSpeechPermission")]
        [property: JsonPropertyName("textToSpeechPermission")] string textToSpeechPermission,
     [property: JsonProperty("epub")]
        [property: JsonPropertyName("epub")] Epub epub,
     [property: JsonProperty("pdf")]
        [property: JsonPropertyName("pdf")] Pdf pdf,
     [property: JsonProperty("webReaderLink")]
        [property: JsonPropertyName("webReaderLink")] string webReaderLink,
     [property: JsonProperty("accessViewStatus")]
        [property: JsonPropertyName("accessViewStatus")] string accessViewStatus,
     [property: JsonProperty("quoteSharingAllowed")]
        [property: JsonPropertyName("quoteSharingAllowed")] bool quoteSharingAllowed
 );

    public record Epub(
        [property: JsonProperty("isAvailable")]
        [property: JsonPropertyName("isAvailable")] bool isAvailable,
        [property: JsonProperty("acsTokenLink")]
        [property: JsonPropertyName("acsTokenLink")] string acsTokenLink
    );

    public record ImageLinks(
        [property: JsonProperty("smallThumbnail")]
        [property: JsonPropertyName("smallThumbnail")] string smallThumbnail,
        [property: JsonProperty("thumbnail")]
        [property: JsonPropertyName("thumbnail")] string thumbnail
    );

    public record IndustryIdentifier(
        [property: JsonProperty("type")]
        [property: JsonPropertyName("type")] string type,
        [property: JsonProperty("identifier")]
        [property: JsonPropertyName("identifier")] string identifier
    );

    public record Item(
        [property: JsonProperty("kind")]
        [property: JsonPropertyName("kind")] string kind,
        [property: JsonProperty("id")]
        [property: JsonPropertyName("id")] string id,
        [property: JsonProperty("etag")]
        [property: JsonPropertyName("etag")] string etag,
        [property: JsonProperty("selfLink")]
        [property: JsonPropertyName("selfLink")] string selfLink,
        [property: JsonProperty("volumeInfo")]
        [property: JsonPropertyName("volumeInfo")] VolumeInfo volumeInfo,
        [property: JsonProperty("saleInfo")]
        [property: JsonPropertyName("saleInfo")] SaleInfo saleInfo,
        [property: JsonProperty("accessInfo")]
        [property: JsonPropertyName("accessInfo")] AccessInfo accessInfo,
        [property: JsonProperty("searchInfo")]
        [property: JsonPropertyName("searchInfo")] SearchInfo searchInfo
    );

    public record ListPrice(
        [property: JsonProperty("amount")]
        [property: JsonPropertyName("amount")] double amount,
        [property: JsonProperty("currencyCode")]
        [property: JsonPropertyName("currencyCode")] string currencyCode,
        [property: JsonProperty("amountInMicros")]
        [property: JsonPropertyName("amountInMicros")] int amountInMicros
    );

    public record Offer(
        [property: JsonProperty("finskyOfferType")]
        [property: JsonPropertyName("finskyOfferType")] int finskyOfferType,
        [property: JsonProperty("listPrice")]
        [property: JsonPropertyName("listPrice")] ListPrice listPrice,
        [property: JsonProperty("retailPrice")]
        [property: JsonPropertyName("retailPrice")] RetailPrice retailPrice,
        [property: JsonProperty("giftable")]
        [property: JsonPropertyName("giftable")] bool giftable
    );

    public record PanelizationSummary(
        [property: JsonProperty("containsEpubBubbles")]
        [property: JsonPropertyName("containsEpubBubbles")] bool containsEpubBubbles,
        [property: JsonProperty("containsImageBubbles")]
        [property: JsonPropertyName("containsImageBubbles")] bool containsImageBubbles
    );

    public record Pdf(
        [property: JsonProperty("isAvailable")]
        [property: JsonPropertyName("isAvailable")] bool isAvailable,
        [property: JsonProperty("acsTokenLink")]
        [property: JsonPropertyName("acsTokenLink")] string acsTokenLink
    );

    public record ReadingModes(
        [property: JsonProperty("text")]
        [property: JsonPropertyName("text")] bool text,
        [property: JsonProperty("image")]
        [property: JsonPropertyName("image")] bool image
    );

    public record RetailPrice(
        [property: JsonProperty("amount")]
        [property: JsonPropertyName("amount")] double amount,
        [property: JsonProperty("currencyCode")]
        [property: JsonPropertyName("currencyCode")] string currencyCode,
        [property: JsonProperty("amountInMicros")]
        [property: JsonPropertyName("amountInMicros")] int amountInMicros
    );

    public record Root(
        [property: JsonProperty("kind")]
        [property: JsonPropertyName("kind")] string kind,
        [property: JsonProperty("totalItems")]
        [property: JsonPropertyName("totalItems")] int totalItems,
        [property: JsonProperty("items")]
        [property: JsonPropertyName("items")] IReadOnlyList<Item> items
    );

    public record SaleInfo(
        [property: JsonProperty("country")]
        [property: JsonPropertyName("country")] string country,
        [property: JsonProperty("saleability")]
        [property: JsonPropertyName("saleability")] string saleability,
        [property: JsonProperty("isEbook")]
        [property: JsonPropertyName("isEbook")] bool isEbook,
        [property: JsonProperty("listPrice")]
        [property: JsonPropertyName("listPrice")] ListPrice listPrice,
        [property: JsonProperty("retailPrice")]
        [property: JsonPropertyName("retailPrice")] RetailPrice retailPrice,
        [property: JsonProperty("buyLink")]
        [property: JsonPropertyName("buyLink")] string buyLink,
        [property: JsonProperty("offers")]
        [property: JsonPropertyName("offers")] IReadOnlyList<Offer> offers
    );

    public record SearchInfo(
        [property: JsonProperty("textSnippet")]
        [property: JsonPropertyName("textSnippet")] string textSnippet
    );

    public record VolumeInfo(
        [property: JsonProperty("title")]
        [property: JsonPropertyName("title")] string title,
        [property: JsonProperty("publishedDate")]
        [property: JsonPropertyName("publishedDate")] string publishedDate,
        [property: JsonProperty("description")]
        [property: JsonPropertyName("description")] string description,
        [property: JsonProperty("industryIdentifiers")]
        [property: JsonPropertyName("industryIdentifiers")] IReadOnlyList<IndustryIdentifier> industryIdentifiers,
        [property: JsonProperty("readingModes")]
        [property: JsonPropertyName("readingModes")] ReadingModes readingModes,
        [property: JsonProperty("pageCount")]
        [property: JsonPropertyName("pageCount")] int pageCount,
        [property: JsonProperty("printType")]
        [property: JsonPropertyName("printType")] string printType,
        [property: JsonProperty("categories")]
        [property: JsonPropertyName("categories")] IReadOnlyList<string> categories,
        [property: JsonProperty("maturityRating")]
        [property: JsonPropertyName("maturityRating")] string maturityRating,
        [property: JsonProperty("allowAnonLogging")]
        [property: JsonPropertyName("allowAnonLogging")] bool allowAnonLogging,
        [property: JsonProperty("contentVersion")]
        [property: JsonPropertyName("contentVersion")] string contentVersion,
        [property: JsonProperty("language")]
        [property: JsonPropertyName("language")] string language,
        [property: JsonProperty("previewLink")]
        [property: JsonPropertyName("previewLink")] string previewLink,
        [property: JsonProperty("infoLink")]
        [property: JsonPropertyName("infoLink")] string infoLink,
        [property: JsonProperty("canonicalVolumeLink")]
        [property: JsonPropertyName("canonicalVolumeLink")] string canonicalVolumeLink,
        [property: JsonProperty("subtitle")]
        [property: JsonPropertyName("subtitle")] string subtitle,
        [property: JsonProperty("authors")]
        [property: JsonPropertyName("authors")] IReadOnlyList<string> authors,
        [property: JsonProperty("publisher")]
        [property: JsonPropertyName("publisher")] string publisher,
        [property: JsonProperty("panelizationSummary")]
        [property: JsonPropertyName("panelizationSummary")] PanelizationSummary panelizationSummary,
        [property: JsonProperty("imageLinks")]
        [property: JsonPropertyName("imageLinks")] ImageLinks imageLinks,
        [property: JsonProperty("averageRating")]
        [property: JsonPropertyName("averageRating")] double? averageRating,
        [property: JsonProperty("ratingsCount")]
        [property: JsonPropertyName("ratingsCount")] int? ratingsCount
    );
}
