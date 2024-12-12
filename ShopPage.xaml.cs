using CrisanMelisaLab7.Models;
using Microsoft.Maui.Devices.Sensors;
using Plugin.LocalNotification;

namespace CrisanMelisaLab7;

public partial class ShopPage : ContentPage
{
	public ShopPage()
	{
        InitializeComponent();
    }

    async void OnSaveButtonClicked(object sender, EventArgs e)
	{ 
		var shop = (Shop)BindingContext; 
		await App.Database.SaveShopAsync(shop); 
		await Navigation.PopAsync();
	}
    async void OnShowMapButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        var address = shop.Adress;

        // Ob?ine loca?iile magazinului
        var locations = await Geocoding.GetLocationsAsync(address);
        var shopLocation = locations?.FirstOrDefault();

        if (shopLocation == null)
        {
            await DisplayAlert("Eroare", "Loca?ia magazinului nu a fost g�sit�.", "OK");
            return;
        }

        // Ob?ine loca?ia curent�
        var myLocation = await Geolocation.GetLocationAsync();
        if (myLocation == null)
        {
            await DisplayAlert("Eroare", "Loca?ia curent� nu este disponibil�.", "OK");
            return;
        }

        // Calculeaz� distan?a
        var distance = myLocation.CalculateDistance(shopLocation, DistanceUnits.Kilometers);

        // Trimite notificare dac� distan?a este mai mic� de 5 km
        if (distance < 5)
        {
            var request = new NotificationRequest
            {
                Title = "Ai de f�cut cump�r�turi �n apropiere!",
                Description = address,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };

            // Trimite notificarea
            LocalNotificationCenter.Current.Show(request);
        }

        // Deschide loca?ia pe hart�
        var options = new MapLaunchOptions { Name = "Magazinul meu preferat" };
        await Map.OpenAsync(shopLocation, options);
    }
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;

        if (shop == null)
        {
            await DisplayAlert("Eroare", "Magazinul nu a fost g�sit.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmare",
            "E?ti sigur c� vrei s� ?tergi acest magazin?", "Da", "Nu");

        if (confirm)
        {
            await App.Database.DeleteShopAsync(shop);
            await Navigation.PopAsync();
        }
    }


}