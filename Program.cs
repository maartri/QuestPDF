// See https://aka.ms/new-console-template for more information


using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Data.Common;
using System.Net;
using System.Reflection;



 namespace PDFA
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var policyDetails = new List<(string name, decimal value)>()
                {
                    ("Current value", 200),
                    ("Pending Tax relief", 100),
                    ("Weekly deposit", 300),
                    ("Monthly payday boost", 3500),
                };

                QuestPDF.Settings.License = LicenseType.Community;
                // code in your main method
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);

                        page.Header().Element(ComposeHeader);

                        page.Content().Element(ComposeContent);


                        //page.Footer().AlignCenter().Text(x =>
                        //{
                        //    x.CurrentPageNumber();
                        //    x.Span(" / ");
                        //    x.TotalPages();
                        //});
                    });
                });

                // instead of the standard way of generating a PDF file
                document.GeneratePdf("hello.pdf");

                // use the following invocation
                document.ShowInPreviewer();

                // optionally, you can specify an HTTP port to communicate with the previewer host (default is 12500)
                document.ShowInPreviewer(12345);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            void ComposeHeader(IContainer container)
            {
                var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                container.Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text($"Invoice #11").Style(titleStyle);

                        column.Item().Text(text =>
                        {
                            text.Span("Issue date: ").SemiBold();
                            text.Span($"1");
                        });

                        column.Item().Text(text =>
                        {
                            text.Span("Due date: ").SemiBold();
                            text.Span($"1");
                        });
                    });

                    row.ConstantItem(100).Height(50).Placeholder();
                });
            }


            void ComposeContent(IContainer container)
            {
                //container.PaddingVertical(20).DefaultTextStyle(x => x.FontFamily("Calibri").FontColor("#006179")).Column(column =>
                //{
                //    column.Item().Text("What the benefits from your All Aussie Adventures Pension might be worth on DD Month YYYY").Bold().FontSize(20).FontColor("#006179");

                //    column.Item().Text("Below is an example of a yearly investment growth rate:");



                //    column.Item().Text("* A negative growth rate after inflation will reduce the buying power of your All Aussie Adventures Personal Pension over time.").FontSize(8);
                //    column.Item();

                //    column.Item().PageBreak();

                //});

                container.PaddingVertical(40).Column(column =>
                {
                    column.Spacing(5);

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Component(new AddressComponent("From", Model.SellerAddress));
                        row.ConstantItem(50);
                        row.RelativeItem().Component(new AddressComponent("For", Model.CustomerAddress));
                    });

                    column.Item().Element(ComposeTable);

                    var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
                    column.Item().AlignRight().Text($"Grand total: {totalPrice}$").FontSize(14);

                    if (!string.IsNullOrWhiteSpace(Model.Comments))
                        column.Item().PaddingTop(25).Element(ComposeComments);
                });


            }
        }
    }

    public class AddressComponent : IComponent
    {
        private string Title { get; }
        private Address Address { get; }

        public AddressComponent(string title, Address address)
        {
            Title = title;
            Address = address;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                column.Item().BorderBottom(1).PaddingBottom(5).Text(Title).SemiBold();

                column.Item().Text(Address.CompanyName);
                column.Item().Text(Address.Street);
                column.Item().Text($"{Address.City}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);
            });
        }
    }
}