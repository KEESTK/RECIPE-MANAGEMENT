# Programmieraufgabe: Rezeptverwaltung als .NET-Bibliothek

## Ziel
Entwickeln Sie eine C#-Bibliothek (.NET), die eine Nutzer- und Rezeptverwaltung implementiert.
Die Daten sollen geeignet persistiert werden. Zusätzlich soll ein einfacher Mechanismus bereitgestellt werden, um die Funktionalität der Bibliothek zu demonstrieren.

## Technische Vorgaben
- Sprache & Framework: C# im .NET-Ökosystem
- Präsentation: Ergänzen Sie ein kleines Zusatzprojekt, das die Funktionalität zeigt.

## Fachliche Anforderungen

### Nutzerverwaltung
- Nur registrierte Benutzer dürfen Rezepte verwalten.
- Eine Benutzerverwaltung ist erforderlich.

### Rezeptverwaltung
- Rezepte können durch einen User angelegt, bearbeitet und gelöscht werden.
- Einschränkungen beim Anlegen:
    - Namen von Rezepten sind global eindeutig (über alle User hinweg).
    - Jedes Rezept benötigt mindestens einen Zubereitungsschritt.
    - Jedes Rezept benötigt mindestens eine Zutat.
    - Jedes Rezept muss mindestens einer Kategorie zugeordnet sein (mehrere sind möglich).
- Zutaten:
    - Zutaten stammen aus einer globalen, user-unabhängigen Zutatenliste.
    - Existiert eine Zutat noch nicht, kann sie hinzugefügt werden.

### Kategorieverwaltung

- Kategorien können angelegt, bearbeitet und gelöscht werden.
- Einschränkung: Kategorienamen sind eindeutig.

### Abfragen & Favoriten
- Alle Rezepte eines bestimmten Users anzeigen.
- Alle Rezepte einer bestimmten Kategorie anzeigen.
- Alle Rezepte mit einer bestimmten Zutat anzeigen.
- Optional: Rezepte anderer Benutzer können als Favorit markiert oder demarkiert werden.
- Optional: Favoriten eines Users können angezeigt werden.

## Erwartete Ergebnisse
- Ein Git-Repository (Link zu öffentlichem Repository oder .Zip-Datei)
- Eine lauffähige Bibliothek mit der Implementierung aller oben beschriebenen Anforderungen.
- Einen Ansatz, um die von der Bibliothek verarbeiteten Daten dauerhaft zu speichern.
- Ein Beispielprojekt, das die Nutzung demonstriert.
- Eine kurze README-Datei mit:
    - Installations-/Startanleitung
    - Übersicht der Architektur
