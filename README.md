# MatfyzWikyToPdf

Program, který stáhne data z Matfyzácké wikypedie a překonvertuje je do LaTeXu.

## Popis

Program, stahuje data z určitých stránek na Matfyzácké wikypedii a poté je konvertuje je do LaTeXu. Lze nastavit, které stránky se budou stahovat, hlavní kapitoly a podkapitoly v souboru Pages. V souboru Conditions.xml lze zapsat jednoduché příkazy na úpravu dat. Výstupem programu jsou soubory s LaTeXem, které je poté potřeba zkonvertovat do pdfka pomocí LaTeX editoru.

## Použití

Pro spuštění programu je nutné mít nainstalovaný pandoc, který překládá wiky syntaxi do LaTeXu a je spuštěn programem pomocí příkazové řádky. A poté, na zpracování výstupu, LaTeX editor (například TeXworks), pomocí kterého potom vygenerujete pdfko.

## Soubory

### Pages.txt

V tomto souboru je seznam stránek na stažení. Vložením mezery před jméno strány se vytvoří podkapitola. Na jeden řádek se vkládá jméno jedné stránky.

### Conditions.xml

V tomto souboru jsou jednoduché příkazy v xml, pomocí kterých se dá upravit LaTeX při generování. Jediný existující příkaz je <replace> do kterého se vkládají údaje "from" a "to", v podobě elementů <from> a <to> nebo v jako atributy do <replace>. Nebo lze nahradit "from" na "startString", do kterého se vloží hodnota od které se text nahradí (Hodnota v "startString" bude také nahrazena.).

### Data.txt

V tomto souboru je jenom url ke stránce na wikypedii, která slouží na export.