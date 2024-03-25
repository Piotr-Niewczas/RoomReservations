# Projekt „RoomReservations”

Strona do rezerwacji pokoi hotelowych zaprezentowana na przykładzie fikcyjnego hotelu "Kaktus Hotel".

#### Interfejs dla Klienta:
- Goście mają dostęp do intuicyjnego interfejsu, gdzie mogą przeglądać ofertę pokoi, sprawdzać ich wyposażenie, ceny i dostępność.
- Mogą dokonywać rezerwacji online, wybierając preferowane daty i opcje.
- Mogą również samodzielnie przedłużyć swój pobyt, jeżeli pozwala na to dostępności pokoi. 

#### Interfejs dla Recepcjonisty:
- Recepcjonista ma dostęp do panelu administracyjnego, gdzie może zarządzać rezerwacjami.
- Może sprawdzać dostępność pokoi, tworzyć nowe rezerwacje, modyfikować istniejące oraz anulować rezerwacje.

#### Interfejs dla Innych Pracowników Hotelu:
- Inni pracownicy, tak jak sprzątacz, również mogą korzystać z systemu, aby wiedzieć, które pokoje należy przygotować lub posprzątać.




## Autorzy
- Frankiewicz Przemysław
- Niewczas Piotr

## Zrzuty ekranu
### Strona główna
![strona główna](https://github.com/Piotr-Niewczas/RoomReservations/assets/74670892/8820d6fc-a02c-45d2-9911-e4cf62d1f2fc)
### Lista pokoi
![Lista pokoi](https://github.com/Piotr-Niewczas/RoomReservations/assets/74670892/3e2443bc-eb40-4aff-9da8-bdef4dbace69)
### Tworzenie rezerwacji
![Tworzenie rezerwacji](https://github.com/Piotr-Niewczas/RoomReservations/assets/74670892/af8eb8c3-9a5e-4845-b1de-19063088f6db)

## Uruchamianie
Wystarczy uruchomić *‘RoomReservations.exe’* i wpisać w przeglądarce adres *‘localhost’*.  Aplikację można wyłączyć przytrzymując *’CTRL-C’* w konsoli.

Należy zwrócić uwagę, aby w tym samym katalogu znajdował się plik *‘hotel.db’* z bazą danych.

Domyślny port aplikacji z portu 80 można zmienić na dowolny w pliku *‘appsettings.json’*.
## Domyślne konta użytkowników

|**Email**|**Hasło**|**Rola**|
| :- | :- | :- |
|**client@c.com**|Client123#|Klient hotelu|
|**receptionist@r.com**|Receptionist123#|Recepcjonista|
|**employee@e.com**|Employee123#|Inny pracownik hotelu|
|**admin@a.com**|Admin123#|Administrator systemu|

## Zastosowane technologie
- ASP.NET Core 8.0
- Entity Framework
- Identity Framework
- Blazor
- MudBlazor

## Źródła
Obrazy, opisy pokoi, oraz strona główna zostały wygenerowane za pomocą usługi Microsoft Copilot https://www.bing.com/chat
