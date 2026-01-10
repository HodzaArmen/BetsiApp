# Betsi

Dobrodošli v projektu **Betsi**. To je spletna aplikacija, razvita v **ASP.NET Core 8.0 Razor Pages**, ki služi kot platforma za prikaz nogometnih ter košarkaških tekem, (trenutno namišljenih) kvot in omogoča oddajo virtualnih stav.

## 1. Začetek projekta

Za delovanje aplikacije potrebujete naslednje orodje:

* **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**.

### A. Kloniranje in namestitev

```bash
# Kloniranje repozitorija
git clone https://github.com/HodzaArmen/BetsiApp
cd BetsiApp

# Namestitev vseh potrebnih paketov
dotnet restore
```

### B. Zagon baze podatkov in seeding
```bash
# 1. Posodobi bazo podatkov in ustvari tabele
dotnet ef database update --context ApplicationDbContext

# 2. Zagon aplikacije
dotnet run
```

Testni uporabniki za prijavo:
- admin@betsi.com
- Geslo: Test123!

- uporabnik@betsi.com
- Geslo: Uporabnik123!

## 2. Struktura Projekta
Aplikacija sledi arhitekturi ASP.NET Core Razor Pages.

- Pages/Index.cshtml - Glavna stran z dinamičnim prikazom tekem, kvot in interaktivnim stavnim lističem (JavaScript/jQuery).
- Services/FootballApiService.cs - Logika za pridobivanje podatkov iz zunanjega API-ja.
- Data/ApplicationDbContext.cs - Glavni kontekst baze podatkov.
- Data/DbInitializer.cs - Logika za avtomatsko vstavljanje testnih podatkov ob zagonu.
- Models/BettingModels.cs - C# modeli za stavno logiko: Odd, BetSlip, BetItem.
- Areas/Identity/ - Generirane Razor Pages za avtentikacijo (Prijava, Registracija, Odjava).

## 3. Nadaljnji razvoj in TODO

### A. Implementacija oddaje stavnega listka z več tekmami
Cilj: Dokončati feature, ki uporabnikom omogoča, da na stavni listek dodajo več kot eno stavo, preden se le-ta zaključi in ga vplačajo.

Status: Ne bo časa...

### B. Implementacija javnih leaderboardov
Cilj: Saj ni pomembno

Status: Niti v sanjah nam ne uspe.
