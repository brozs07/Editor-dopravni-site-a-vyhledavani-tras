# Editor dopravní sítě a vyhledávání tras
Aplikace bude sloužit jako interaktivní editor grafů a navigační systém v dopravní síti (např. metro, autobusy či vlaky).

Uživatel bude moci přímo na vektorové plátno pomocí myši vykreslovat uzly (zastávky/stanice) a propojovat je obousměrnými i jednosměrnými hranami (spojení s definovanou délkou v kilometrech). Aplikace musí umět na základě zadaného počátečního a cílového bodu vyhledat nejkratší trasu pomocí Dijkstrova algoritmu a tuto trasu graficky zvýraznit na plátně včetně plynulé nebo krokovatelné animace průběhu hledání (s možností regulace rychlosti).

Součástí bude možnost simulace výluky (dočasné uzavření spoje nebo stanice) s okamžitým přepočtem alternativní trasy. Data sítě bude možné ukládat a načítat ze strukturovaného souboru ve formátu JSON nebo XML. Uživatelské rozhraní bude podporovat plynulé přibližování (zoom), posun plátna (pan) a kompletní responzivitu.
