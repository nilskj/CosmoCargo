# 🚀 CosmoCargo™ – Intergalaktisk Fraktcentral

## 🪐 Bakgrund

**CosmoCargo™** är den ledande aktören inom rymdlogistik, med leveranser till över 9000 rymdstationer. Ett sofistikerat bokningssystem används av **kunder**, **piloter** och **administratörer**.

## 🛠️ Teknisk Stack

### Frontend
- React
- TypeScript
- Next.js
- Tanstack Query
- Tailwind CSS
- Mocha (för testning)

### Backend
- .NET
- Minimal API
- REST
- Entity Framework Core
- PostgreSQL

### Infrastruktur
- Docker Compose
- Infrastructure as Code (IaC)

## 🚀 Kom igång

### Förutsättningar
- Docker och Docker Compose
- Node.js (v18+)
- .NET SDK 8.0+

### Installation

1. Klona repositoryt:
```bash
git clone https://github.com/ditt-användarnamn/cosmocargo.git
cd cosmocargo
```

2. Starta hela applikationen med Docker Compose:
```bash
docker-compose up -d
```

3. Applikationen är nu tillgänglig på:
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000

## 📄 Funktionalitet

Se den fullständiga specifikationen för detaljer om systemets funktionalitet, inklusive:
- Inloggning för kunder, piloter och administratörer
- Fraktbokning och spårning
- Tullformulär och riskbedömning
- Administrativ hantering

## 🧪 Utveckling

### Köra frontend separat
```bash
cd frontend
npm install
npm run dev
```

### Köra backend separat
```bash
cd backend
dotnet restore
dotnet run
```

### Köra tester
```bash
# Frontend tester
cd frontend
npm test

# Backend tester
cd backend
dotnet test
```

## 📁 Projektstruktur

```
cosmocargo/
├── frontend/           # Next.js React-applikation
├── backend/            # .NET Minimal API
├── infrastructure/     # IaC-filer
├── docker-compose.yml  # Docker Compose-konfiguration
└── README.md           # Projektdokumentation
```

## 🛠️ Funktionella krav

### 🎨 Frontend

#### 🔐 Inloggning & Roller
- Inloggning/registrering för tre roller: **Kund**, **Pilot**, **Admin**

#### 👩‍🚀 Kundvy
- Skapa en frakt via formulär (avsändare, mottagare, vikt, kategori, prioritet)
- Se pågående och tidigare leveranser
- Spåra paket i realtid (simulerad status)

#### 🚀 Pilotvy
- Se tilldelade frakter (status, datum, destination)
- Uppdatera status: *påbörjad*, *pågående*, *levererad*

#### 🧑‍💼 Admin Dashboard
- Se alla frakter med filterfunktionalitet
- Tilldela pilot till frakt
- Ändra status till "Lost in Black Hole"

---

## 📄 Tullformulär: "Galactic Cargo Declaration"

Universum inför intergalaktiska tullar – ett nytt system krävs.

### Funktionalitet
- **Kund**: Skickar in tullformulär vid bokning
- **Admin**: Granskar formuläret
- **Pilot**: Ser om frakten är högriskklassad

---

## 🧩 Formulärfält

| Fält | Typ | Beskrivning |
|------|-----|-------------|
| `shipmentId` | UUID | Referens till frakten |
| `containsLifeforms` | Boolean | Innehåller levande varelser |
| `lifeformType` | Text | (Om ja) Beskrivning av art, intelligens, riskklass |
| `isPlasmaActive` | Boolean | Innehåller plasma-aktiva material |
| `plasmaStabilityLevel` | Number (1–10) | Stabilitetsskala (om plasmaaktiv) |
| `originPlanetLawsConfirmed` | Checkbox | Intygande om laglig export |
| `quarantineRequired` | Boolean | Kräver karantänzon vid ankomst |
| `customsNotes` | Textarea | Frivillig kommentar |
| `submittedBy` | Auto | Användare kopplas automatiskt |

---

## 🛡️ Valideringsregler

- `containsLifeforms === true` ⇒ `lifeformType` är obligatoriskt
- `isPlasmaActive === true` ⇒ `plasmaStabilityLevel` måste anges (1–10)
- `plasmaStabilityLevel < 4` ⇒ `quarantineRequired` måste vara true
- `originPlanetLawsConfirmed` måste vara ikryssad
- Textfält har begränsningar (säkerhet)

---

## 💥 Automatiserad Riskbedömning

Bygg en funktion som klassificerar risknivå baserat på tullformulär.

### Riskklassificering

| Villkor | Risknivå |
|---------|----------|
| Plasmaaktiv + stabilitet < 5 + livsform | 🔴 Kritisk |
| Livsform + okänd art | 🔶 Hög |
| Plasmaaktiv med stabilitet 5–7 | 🟡 Medel |
| Inga specialegenskaper | 🟢 Låg |

---

## 🔧 Backend

### Funktionalitet
- Autentisering via JWT eller liknande
- REST API eller GraphQL för:
  - Fraktbokning
  - Fraktstatus
  - Roller & auth
  - Tullformulär

### Fraktdata innehåller
- ID, avsändare, destination, vikt, typ, risknivå, status

### Statusuppdatering
- Simulerad statusförändring över tid (t.ex. cron-jobb)

---

## 🗄️ Databas

**Förslag:** PostgreSQL eller MongoDB

### Modeller
- `User`
- `Shipment`
- `TollForm`

---

## ⚙️ Infrastruktur & DevOps

- Dockerisera hela stacken (frontend, backend, databas)
- Använd `docker-compose.yml` för enkel uppstart
- CI/CD med GitHub Actions:
  - Linting
  - Tester (minst enhetstester på backend)
  - Bygg & deploy till moln (Railway, Vercel etc.)

---

## 🧪 Setup & Instruktioner

- Lokalt uppstart: se `README.md`
- Användning av API
- Rollbaserad åtkomst

---

## ⭐ Bonusutmaningar

- Realtidsuppdatering via WebSockets
- Visuell rymdkarta med destinationer
- "Försäkra" leverans (extra avgift & spårning)
- Statistik till admin (ex. mest trafikerade planeter)
- Chatbot – ex. "Vart är mitt paket?"

---

## ✅ Bedömningskriterier

- Funktionalitet & kravuppfyllnad
- Responsiv design
- Arkitektur & kodkvalitet
- UX/UI
- DevOps & struktur
- Dokumentation & onboarding