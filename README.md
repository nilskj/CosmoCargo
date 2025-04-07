
```markdown
# 🚀 Kodtest: CosmoCargo™ – Intergalaktisk Fraktcentral

## 🪐 Bakgrund

**CosmoCargo™** är den ledande aktören inom rymdlogistik, med leveranser till över 9000 rymdstationer. Ett sofistikerat bokningssystem används av **kunder**, **piloter** och **administratörer**. Du har blivit anlitad för att bygga en ny feature med fokus på användarvänlighet, tydlig struktur och god kodkvalitet.

---

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
- Ändra status till “Lost in Black Hole”

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
- “Försäkra” leverans (extra avgift & spårning)
- Statistik till admin (ex. mest trafikerade planeter)
- Chatbot – ex. “Vart är mitt paket?”

---

## ✅ Bedömningskriterier

- Funktionalitet & kravuppfyllnad
- Responsiv design
- Arkitektur & kodkvalitet
- UX/UI
- DevOps & struktur
- Dokumentation & onboarding
```