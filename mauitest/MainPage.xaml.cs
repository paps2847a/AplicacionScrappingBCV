using mauitest.Services;
using System.Threading.Tasks;

namespace mauitest
{
    public partial class MainPage : ContentPage
    {
        private readonly IBCVScrapperService _bCVScrapperService;
        private bool _isUpdating = false;
        private string _usedCurrency = "USD";
        private decimal selectedCurrencyPrice = 0;

        public MainPage(IBCVScrapperService bCVScrapperService)
        {
            InitializeComponent();
            _bCVScrapperService = bCVScrapperService;
        }

        private async void CopyDolares(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(dolares.Text))
            {
                await Clipboard.SetTextAsync(dolares.Text);
                await DisplayAlert("Copiado", "Se copió el valor de Dólares", "OK");
            }
        }

        private async void CopyBolivares(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(bolivares.Text))
            {
                await Clipboard.SetTextAsync(bolivares.Text);
                await DisplayAlert("Copiado", "Se copió el valor de Bolívares", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            selectedCurrencyPrice = await _bCVScrapperService.GetBCVDolar(_usedCurrency);

            dolares.Text = "1";
            bolivares.Text = selectedCurrencyPrice.ToString();
            picker.SelectedIndex = 0;
        }

        private async void CleanAll(object sender, EventArgs e)
        {
            selectedCurrencyPrice = await _bCVScrapperService.GetBCVDolar(_usedCurrency);

            dolares.Text = "1";
            bolivares.Text = selectedCurrencyPrice.ToString();
        }

        private async void ChangeOfCurrency(object sender, EventArgs e)
        {
            _usedCurrency = picker.SelectedIndex == 0 ? "USD" : "EURO";
            selectedCurrencyPrice = (await _bCVScrapperService.GetBCVDolar(_usedCurrency));

            dolares.Text = "1";
            bolivares.Text = selectedCurrencyPrice.ToString();
        }

        private void dolares_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;

            _isUpdating = true;

            if (string.IsNullOrWhiteSpace(dolares.Text))
            {
                bolivares.Text = "0";
                _isUpdating = false;
                return;
            }

            if (!decimal.TryParse(dolares.Text, out decimal dolaresDecimal))
            {
                bolivares.Text = "0";
                _isUpdating = false;
                return;
            }

            var bolivaresDecimal = decimal.Round(dolaresDecimal * selectedCurrencyPrice, 2);

            bolivares.Text = bolivaresDecimal.ToString();
            _isUpdating = false;
        }

        private void bolivares_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;

            _isUpdating = true;

            if (string.IsNullOrWhiteSpace(bolivares.Text))
            {
                dolares.Text = "0";
                _isUpdating = false;
                return;
            }

            if (!decimal.TryParse(bolivares.Text, out decimal bolivaresDecimal))
            {
                dolares.Text = "0";
                _isUpdating = false;
                return;
            }

            var dolaresDecimal = decimal.Round(bolivaresDecimal / selectedCurrencyPrice, 2);

            dolares.Text = dolaresDecimal.ToString();
            _isUpdating = false;
        }


    }
}
