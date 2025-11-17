namespace Requalify.Hateoas
{
    public abstract class ResourceResponse
    {
        public List<LinkDto> Links { get; set; } = new();

        public void AddLink(string rel, string? href, string method)
        {
            Links.Add(new LinkDto(rel, href, method));
        }
    }
}
