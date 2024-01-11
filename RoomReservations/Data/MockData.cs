using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public static class MockData
{
    private static readonly List<UserData> MockUsers =
    [
        new UserData("Admin123#", "admin@a.com", RoleNames.Admin, "Admin", "Admin"),
        new UserData("Receptionist123#", "receptionist@r.com", RoleNames.Receptionist, "Receptionist", "Receptionist"),
        new UserData("Employee123#", "employee@e.com", RoleNames.Employee, "John", "Worker"),
        new UserData("Client123#", "client@c.com", RoleNames.Client, "Mr", "Moneyhands", "123456789")
    ];

    private static readonly List<Room> MockRooms =
    [
        new Room
        {
            Id = 1,
            Name = "Niebieska laguna",
            Description =
                "W tym pokoju poczujesz się jak w bajce. Niebieskie ściany otulą Cię spokojem i harmonią. Turkusowe łóżko zaprosi Cię do snu pełnego przygód. Obraz na ścianie przeniesie Cię do egzotycznego świata kaktusów i gór. Kapelusz i kaktus obok łóżka dodadzą Ci meksykańskiego stylu i humoru. Abstrakcyjna sztuka ścienna pobudzi Twoją kreatywność i wyobraźnię. Jasny dywan zmiękczy Twoje kroki i sprawi, że poczujesz się jak w domu.\n\nTen pokój jest nie tylko piękny, ale także komfortowy. Ma wszystko, czego potrzebujesz do wypoczynku i pracy. Łazienka, telewizor, lodówka i Wi-Fi to tylko niektóre z udogodnień, które oferuje. Z okien możesz podziwiać widok na miasto i park, który zachwyci Cię swoim urokiem.\n",
            Capacity = 2,
            PricePerNight = 350,
            Location = "Parter",
            ImageUrl = "img/rooms/laguna.jpeg"
        },

        new Room
        {
            Id = 2,
            Name = "Świetlna pustynia",
            Description =
                "Ten pokój jest jak oaza spokoju i piękna. Duże białe łóżko zaprasza Cię do odpoczynku i relaksu. Nad nim wisi olbrzymi, kolorowy obraz przedstawiający pustynię z kaktusami. Obraz ten jest jak okno do innego świata, pełnego magii i przyrody. Ściany są pomalowane na ciepły, ziemisty kolor, który tworzy przytulną atmosferę. Przy oknie stoi mała, zielona roślina, która dodaje życia i świeżości. Ten pokój jest nie tylko ładny, ale także funkcjonalny. Ma wszystko, czego potrzebujesz do komfortowego pobytu. Łazienka, telewizor, lodówka i Wi-Fi to tylko niektóre z udogodnień, które oferuje. Z okna możesz podziwiać widok na miasto i park, który zachwyci Cię swoim urokiem.",
            Capacity = 2,
            PricePerNight = 500,
            Location = "Piętro 1",
            ImageUrl = "img/rooms/pustynia.jpeg"
        },

        new Room
        {
            Id = 3,
            Name = "Kaktusowo-arbuzowo",
            Description =
                "W tym pokoju zanurzysz się w kolorach i sztuce. Ściany i sufit są malowane jak pustynia z kaktusami. Łóżko jest przytulne i pasuje do stylu południowo-zachodniego. Na ścianie wiszą różne obrazy, które dodają uroku. Przy oknie jest stolik i krzesła, gdzie możesz odpoczywać. Wokół pokoju są doniczki z kaktusami, które dopełniają temat.",
            Capacity = 4,
            PricePerNight = 300,
            Location = "Parter",
            ImageUrl = "img/rooms/arbuz.jpeg"
        },

        new Room
        {
            Id = 4,
            Name = "Wnętrze tęczy",
            Description =
                "Ten pokój jest jak tęcza kolorów i wzorów. Ściany są ozdobione złożonymi, kolorowymi malowidłami ścienne przedstawiającymi różne kształty i wzory. Dwa łóżka z białą pościelą i wielobarwnymi, pasiastymi kocami są ustawione obok siebie. Duże okno z łukowatym wierzchołkiem i ozdobnymi stolarkami zajmuje środek tylnej ściany, oferując widok na zieleń na zewnątrz. Z żółtego sufitu zwisa żyrandol, rzucając światło, które podkreśla ciepłe tony pokoju. Pasiasty dywan pokrywa podłogę, uzupełniając ogólną kolorystykę pokoju. Przy oknie są dwa krzesła i stolik, tworząc małą strefę wypoczynkową.",
            Capacity = 2,
            PricePerNight = 450,
            Location = "Piętro 1",
            ImageUrl = "img/rooms/tecza.jpeg"
        },

        new Room
        {
            Id = 5,
            Name = "Żywa Oaza",
            Description =
                "Ten pokój jest jak żywa oaza kolorów i wzorów. Ściany są pomalowane na żywe kolory, takie jak pomarańczowy, żółty i błękitny, tworząc ciepłą i przyjazną atmosferę. Nad łóżkiem wisi duże, kolorowe mandala, dodające do pokoju artystycznego charakteru. Łóżko jest ozdobione czerwoną narzutą i poduszkami, które mają złożone wzory, pasujące do ogólnego stylu pokoju. Po obu stronach łóżka są błękitne nocne stoliki, na których stoją nowoczesne lampy, zapewniające miękkie oświetlenie. Na podłodze leży wzorzysty dywan z geometrycznymi wzorami, dodający tekstury i zainteresowania wizualnego. Łukowate drzwi po jednej stronie pokoju prowadzą do innego pomieszczenia, zwiększając urok architektoniczny tej żywej oazy.",
            Capacity = 4,
            PricePerNight = 800,
            Location = "Piętro 2",
            ImageUrl = "img/rooms/oaza.jpeg"
        },

        new Room
        {
            Id = 6,
            Name = "Sztuka i Rzeczywistość",
            Description =
                "Ten pokój jest jak marzenie, w którym sztuka i rzeczywistość się łączą. Ściany są ozdobione malowidłem, które bezproblemowo łączy się z elementami architektonicznymi, tworząc iluzję rozległego, fantastycznego krajobrazu. Dwa białe łóżka są symetrycznie ustawione po obu stronach pokoju. Przy każdym łóżku jest drewniany stolik z doniczką rośliny, która dodaje zieleni. Kolorowy dywan prowadzi do łukowatych drzwi, które są namalowane jako część malowidła. Sufit jest pomalowany na ciepły, brzoskwiniowy kolor, który pasuje do ogólnego stylu pokoju.",
            Capacity = 4,
            PricePerNight = 600,
            Location = "Parter",
            ImageUrl = "img/rooms/rzeczywistosc.jpeg"
        },

        new Room
        {
            Id = 7,
            Name = "Kaktusowa Przytulanka",
            Description =
                "Ten pokój jest jak kaktusowa przytulanka, która otula Cię kolorami i wzorami. Ściany są pomalowane na dwa kolory: delikatny, koralowy róż i stonowany błękit, tworząc ciepłą i przyjazną atmosferę. Nad łóżkiem wiszą trzy kolorowe, abstrakcyjne obrazy, każdy z innymi kształtami i wzorami geometrycznymi. Łóżko jest starannie zaścielone białą pościelą i pomarańczowymi poduszkami, które dodają akcentu kolorystycznego. Po lewej stronie łóżka jest zbiór doniczkowych kaktusów o różnych wysokościach, dodających zieleni i naturalnego akcentu. Mały, czarny stolik z lampą i doniczkową rośliną stoi obok łóżka. Przed łóżkiem jest niski, drewniany stolik z książkami i ozdobnymi przedmiotami na dywanie z geometrycznymi wzorami, które nawiązują do elementów na obrazach na ścianach.",
            Capacity = 2,
            PricePerNight = 300,
            Location = "Parter",
            ImageUrl = "img/rooms/przytulanka.jpeg"
        },

        new Room
        {
            Id = 8,
            Name = "Fantastyczny Krajobraz",
            Description =
                "Ten pokój jest jak fantastyczny krajobraz, który przenosi Cię do innego wymiaru. Ściana za łóżkiem jest ozdobiona wielkim, kolorowym malowidłem przedstawiającym niezwykły pejzaż z budynkami, schodami, chmurami i roślinnością. Malowidło to jest podzielone na trzy panele. Łóżko jest nowoczesne z białą pościelą i poduszkami; ma minimalistyczny design i jest umieszczone centralnie w pokoju. Po obu stronach pokoju są stylowe krzesła o nowoczesnym kształcie; jedno z nich ma obok mały, okrągły stolik z wazonem z kwiatami. Ściany są pomalowane na ciepły, brzoskwiniowy kolor, który pasuje do kolorów na malowidłach. Po obu stronach łóżka wiszą wiszące lampy, które zapewniają miękkie oświetlenie pokoju; dodatkowe oprawy oświetleniowe są zamontowane na suficie. Na podłodze leży wzorzysty dywan, który dodaje tekstury do drewnianej podłogi.",
            Capacity = 2,
            PricePerNight = 600,
            Location = "Piętro 2",
            ImageUrl = "img/rooms/krajobraz.jpeg"
        },

        new Room
        {
            Id = 9,
            Name = "Ciepły i Przytulny",
            Description =
                "Ten pokój jest jak ciepły i przytulny dom, który otacza Cię kolorami i roślinami. Ściany są pomalowane na ciepłe tony pomarańczowego i błękitnego, tworząc przyjazną atmosferę. Na łóżku leżą wzorzyste poduszki i pasiasty koc, które dodają uroku i komfortu. Nad łóżkiem wiszą obrazy z kaktusami na pomarańczowym tle, które nawiązują do meksykańskiego stylu. Po prawej stronie łóżka jest kolejny kolorowy, abstrakcyjny obraz na błękitnej ścianie. Pokój jest umeblowany nowocześnie, z tapicerowanymi krzesłami i małym stolikiem z wazonem z czerwonymi kwiatami. Wokół pokoju są doniczki z zielonymi roślinami, które dodają życia i świeżości. Wiszące lampy z ozdobnymi wzorami rzucają miękkie światło na cały pokój. Łukowate okna wpuszczają naturalne światło do pokoju, podkreślając jego ciepło i gościnność.",
            Capacity = 4,
            PricePerNight = 500,
            Location = "Piętro 1",
            ImageUrl = "img/rooms/cieply.jpeg"
        },

        new Room
        {
            Id = 10,
            Name = "Słoneczny Raj",
            Description =
                "Ten pokój jest jak słoneczny raj, który otacza Cię kolorami i radością. Ściany są pomalowane na jasny, żółty kolor, który tworzy pogodną atmosferę. Nad łóżkiem wiszą trzy kolorowe, kwadratowe obrazy, każdy z innym motywem kwiatowym. Łóżko jest przytulne i kolorowe, z białą pościelą i poduszkami w różnych odcieniach żółtego. Po obu stronach łóżka są drewniane stoliki, na których stoją lampy i doniczki z kwiatami. Na podłodze leży biały dywan, który dodaje miękkości i czystości. Przy oknie jest narożna sofa, na której możesz odpoczywać i podziwiać widok na ogród. Na sofie leżą kolorowe poduszki i koc, które dodają uroku i komfortu.",
            Capacity = 2,
            PricePerNight = 350,
            Location = "Piętro 2",
            ImageUrl = "img/rooms/raj.jpeg"
        }
    ];

    public static void AddMockDataIfNonePresent(ApplicationDbContext dbContext)
    {
        if (dbContext.Rooms.Any()) return;
        dbContext.Rooms.AddRange(MockRooms);
        dbContext.SaveChanges();
    }

    public static async Task AddMockUsersIfNonePresent(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.Users.AnyAsync()) return; // If there are any users, don't add mock users

        foreach (var user in MockUsers)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = user.Email,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            var createResult = await userManager.CreateAsync(applicationUser, user.Password);
            if (!createResult.Succeeded) throw new Exception($"Failed to create user {user.Email}");
            await userManager.AddToRoleAsync(applicationUser, user.Role);
        }
    }

    private class UserData(
        string password,
        string email,
        string role,
        string firstName,
        string lastName,
        string? phoneNumber = null)
    {
        public string Password { get; } = password;
        public string Email { get; } = email;
        public string Role { get; } = role;
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
        public string? PhoneNumber { get; } = phoneNumber;
    }
}