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
            await DisplayAlert("Eroare", "Loca?ia magazinului nu a fost gãsitã.", "OK");
            return;
        }

        // Ob?ine loca?ia curentã
        var myLocation = await Geolocation.GetLocationAsync();
        if (myLocation == null)
        {
            await DisplayAlert("Eroare", "Loca?ia curentã nu este disponibilã.", "OK");
            return;
        }

        // Calculeazã distan?a
        var distance = myLocation.CalculateDistance(shopLocation, DistanceUnits.Kilometers);

        // Trimite notificare dacã distan?a este mai micã de 5 km
        if (distance < 5)
        {
            var request = new NotificationRequest
            {
                Title = "Ai de fãcut cumpãrãturi în apropiere!",
                Description = address,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };

            // Trimite notificarea
            LocalNotificationCenter.Current.Show(request);
        }

        // Deschide loca?ia pe hartã
        var options = new MapLaunchOptions { Name = "Magazinul meu preferat" };
        await Map.OpenAsync(shopLocation, options);
    }
    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;

        if (shop == null)
        {
            await DisplayAlert("Eroare", "Magazinul nu a fost gãsit.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Confirmare",
            "E?ti sigur cã vrei sã ?tergi acest magazin?", "Da", "Nu");

        if (confirm)
        {
            await App.Database.DeleteShopAsync(shop);
            await Navigation.PopAsync();
        }
    }


}