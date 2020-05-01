# Movie Manager Razor Pages (Teil 1)

## Lernziele

* ASP.NET Core Razor Pages
* Entity Framework Core
* Muster: UnitOfWork, Repositories


## Core

Die Entitätsklassen sind bereits angelegt. Auch die Annotationen zur Definition (inkl. der Migrationen) der Datenbank sind bereits implementiert.

## Klassendiagramm

Die Klasse `Movie` verwaltet die Informationen zu einem konkreten Film inkl. dem Verweis auf die Kategorie (`Category`) des Films.

![Klassendiagramm](./images/00_classdiagram.png)

Im Core-Layer sind die Contracts für die Repositories bedarfsgerecht zu erweitern. Die leeren Interfaces sind bereits vorgegeben.

## Persistierung

Die Persistierung besteht bereits und ist im Projekt `MovieManager.Persistence` implementiert.

## Import

Die Logik zum Einlesen der Movies (inkl. Categories) ist bereits im Projekt `MovieManager.ImportConsole` implementiert.

## ASP.NET Core Razor Pages

Implementieren Sie zur bestehenden Persistenzschicht eine WebApi analog zum [Live-Coding](https://github.com/jfuerlinger/csharp_livecoding_ef_uow_razorpages-part1).

Erstellen Sie ein geeignetes ASP.NET Core Projekt (inkl. Abhängigkeiten) mit der Bezeichnung `MovieManager.Web`.

Verwenden Sie Dependency Injection um die `UnitOfWork` in den `PageModels` verwenden zu können!

Implementieren Sie zumindest folgende zwei Razor Pages:

### Page "Overview"

#### Anforderungen

1. Stellen Sie alle Kategorien in einer Tabelle übersichtlich dar.
1. Spalten der Tabelle
   * Kategoriename
   * Anzahl der Filme
   * Durchschnittliche Dauer der Filme
   * Schaltfläche um zu den Details der Kategorie zu navigieren (Label: „Details“)
1. Sortierung der Tabelle
   * Anzahl der Filme der Kategorie (absteigend)


### Page "Category Details"

#### Anforderungen

1. Name der Kategorie als Überschrift (`<h1>…</h1>`)
2. Stellen Sie alle Filme der entsprechenden Katgegorie in einer Tabelle übersichtlich dar.
3. Spalten der Tabelle
   * Filmtitel
   * Dauer
   * Erscheinungsjahr
4. Sortierung der Tabelle
   * Filmtitel (aufsteigend)
5. Schaltfläche um wieder zur Page „Overview“ zurückzukommen.

## Hinweise
- Verwenden Sie dort wo sinnvoll DataTransferObjects
- Achten Sie eine korrekte Schichtentrennung (Core, Persistence und Web)
- Verwenden Sie das UnitOfWork-Muster
- Dependency Injection (IoC) verwenden
- Erweitern Sie wo notwendig die Repositories