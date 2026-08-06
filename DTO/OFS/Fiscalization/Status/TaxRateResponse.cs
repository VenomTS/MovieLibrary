namespace DTO.OFS.Fiscalization.Status;

public class TaxRateResponse
{
    public int GroupId { get; set; }
    public List<TaxCategoryResponse> TaxCategories { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
}