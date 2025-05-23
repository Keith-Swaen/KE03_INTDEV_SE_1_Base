# KE03_INTDEV_SE_1_Base

## Waarom mijn commits verdwenen zijn

Tijdens het werken met Git heb ik per ongeluk `git rebase` en andere commands gebruikt zonder volledig te begrijpen wat er gebeurde. Hierdoor zijn sommige commits tijdelijk verdwenen uit mijn zichtbare geschiedenis.

Ik heb geprobeerd mijn werk terug te halen via `git reflog` en `git fsck`. Dit was een leerervaring over voorzichtig omgaan met Git, vooral bij rebase en push.

---

## Project opstarten

1. Open een terminal in de rootfolder van de oplossing (de folder waar je `.sln` bestand staat).
2. Voer het volgende uit om alle NuGet-pakketten van alle projecten te herstellen:
    ```bash
    dotnet restore
    ```
3. Navigeer naar de projectmap waar de webapplicatie staat:
    ```bash
    cd KE03_INTDEV_SE_1_Base
    ```
4. Start de applicatie:
    ```bash
    dotnet run
    ```
5. Open je browser en ga naar het adres dat in de console wordt getoond.


---

*Keith Swaen*
