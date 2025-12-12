# Betsi

Dobrodošli v projektu **Betsi**. To je spletna aplikacija, razvita v **ASP.NET Core 8.0 Razor Pages**, ki služi kot platforma za prikaz nogometnih tekem, kvot in omogoča oddajo virtualnih stav.

## 1. Začetek in Zagon Projekta

Za delovanje aplikacije potrebujete naslednje orodje:

* **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** ali novejše.

### A. Kloniranje in Namestitev

```bash
# Kloniranje repozitorija
git clone https://github.com/HodzaArmen/BetsiApp
cd BetsiApp

# Namestitev vseh potrebnih paketov
dotnet restore
```

### B. Zagon Baze Podatkov in Seeding
```bash
# 1. Posodobi bazo podatkov in ustvari tabele (za Identity in Stave)
dotnet ef database update --context ApplicationDbContext

# 2. Zagon aplikacije
dotnet run
```

Testni Uporabniki za Prijavo:
- admin@betsi.com
- Geslo: Test123!

- uporabnik@betsi.com
- Geslo: Uporabnik123!

## 2. Struktura Projekta in Ključne Komponente
Aplikacija sledi arhitekturi ASP.NET Core Razor Pages in principu ločevanja skrbi.

- Pages/Index.cshtml - Glavna stran z dinamičnim prikazom tekem, kvot in interaktivnim stavnim lističem (JavaScript/jQuery).
- Services/FootballApiService.cs - Logika za pridobivanje podatkov iz zunanjega API-ja.
- Data/ApplicationDbContext.cs - Glavni kontekst baze podatkov.
- Data/DbInitializer.cs - Logika za avtomatsko vstavljanje testnih podatkov ob zagonu.
- Models/BettingModels.cs - C# modeli za stavno logiko: Odd, BetSlip, BetItem.
- Areas/Identity/ - Generirane Razor Pages za avtentikacijo (Prijava, Registracija, Odjava).

## 3. Nadaljnji Razvoj in TODO
Glavni manjkajoči del je strežniška logika za oddajo stav in vzdrževanje zgodovine.

### A. Implementacija Oddaje Stave (MVP)
Cilj: Dokončati logiko za shranjevanje.

Task: V Pages/Index.cshtml.cs implementirati OnPostPlaceBetAsync (ali podoben Handler), ki prejme podatke stavnega lističa iz front-enda in jih shrani v bazo podatkov kot entiteti BetSlip in BetItem, povezani s prijavljenim uporabnikom.

### B. Upravljanje Zgodovine Stav
Cilj: Omogočiti uporabnikom pregled njihovih stav.

Task: Ustvariti novo stran (npr. /BetSlips/Index), ki bo iz baze podatkov brala in prikazovala vse BetSlip entitete, oddane s strani prijavljenega uporabnika.

### C. Reševanje Stav (Settlement Logic)
Cilj: Posodabljanje statusa stav po koncu tekme.

Task: Implementirati logiko, ki na podlagi končnih rezultatov, pridobljenih iz zunanjega API-ja, posodobi polje Status v tabeli BetSlip (npr. na WON ali LOST).
