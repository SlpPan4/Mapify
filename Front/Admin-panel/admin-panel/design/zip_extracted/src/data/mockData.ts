export type Side = "Attack" | "Defense";
export type Status = "pending" | "approved" | "rejected";

export interface Submission {
  id: string;
  name: string;
  side: Side;
  map: string;
  categories: string[];
  operators: string[];
  description: string;
  videoUrl: string;
  submittedAt: string;
  submittedBy: string;
  status: Status;
  type: "strat" | "category";
}

export interface Strat {
  id: string;
  name: string;
  side: Side;
  map: string;
  categories: string[];
  operators: string[];
  description: string;
  videoUrl: string;
  createdAt: string;
}

export interface Category {
  id: string;
  name: string;
  side: Side;
  description: string;
  stratCount: number;
  createdAt: string;
}

export const MAPS = [
  "Clubhouse", "Bank", "Consulate", "Kafe Dostoyevsky", "Coastline",
  "Chalet", "Border", "Outback", "Villa", "Lair"
];

export const ATTACK_OPERATORS = ["Ash", "Thermite", "Hibana", "Sledge", "Twitch", "Thatcher", "Fuze", "Iana", "Nomad", "Zero"];
export const DEFENSE_OPERATORS = ["Bandit", "Jäger", "Rook", "Echo", "Maestro", "Pulse", "Vigil", "Frost", "Lesion", "Kapkan"];

export const CATEGORIES = ["Flank", "Rotate", "Setup", "Anchor", "Roam", "Intel", "Breach", "Spawn Peek", "Vertical", "Rappel"];

export const mockSubmissions: Submission[] = [
  {
    id: "SUB-001",
    name: "Clubhouse Bar Rotate Smoke",
    side: "Attack",
    map: "Clubhouse",
    categories: ["Rotate", "Breach"],
    operators: ["Ash", "Thermite"],
    description: "Use Thermite to breach the bar wall from garage, then push through with Ash. Creates a fast rotate that catches defenders off-guard during retake.",
    videoUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    submittedAt: "2026-08-21T09:14:00Z",
    submittedBy: "SiegeProGamer",
    status: "pending",
    type: "strat",
  },
  {
    id: "SUB-002",
    name: "Bank CEO Anchor Hold",
    side: "Defense",
    map: "Bank",
    categories: ["Anchor", "Intel"],
    operators: ["Echo", "Maestro"],
    description: "Two-man anchor setup in CEO using Echo drone for intel denial and Maestro camera for long-range deterrence. Hard to displace without hard breach.",
    videoUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    submittedAt: "2026-08-21T08:47:00Z",
    submittedBy: "DefenseKing99",
    status: "pending",
    type: "strat",
  },
  {
    id: "SUB-003",
    name: "Consulate Spawn Peek Lobby",
    side: "Attack",
    map: "Consulate",
    categories: ["Spawn Peek"],
    operators: ["Ash"],
    description: "Aggressive Ash spawn peek through the lobby windows on round start. High risk, high reward — catches roamers and secures early information.",
    videoUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    submittedAt: "2026-08-21T07:22:00Z",
    submittedBy: "Xeno_Siege",
    status: "pending",
    type: "strat",
  },
  {
    id: "SUB-004",
    name: "Kafe Roam Denial Setup",
    side: "Defense",
    map: "Kafe Dostoyevsky",
    categories: ["Setup", "Roam"],
    operators: ["Frost", "Kapkan"],
    description: "Frost traps at key roaming corridors combined with Kapkan EMDs at window entry points. Forces attackers to slow-clear, buying time for anchors.",
    videoUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    submittedAt: "2026-08-20T21:55:00Z",
    submittedBy: "TacticalMind",
    status: "pending",
    type: "strat",
  },
  {
    id: "SUB-005",
    name: "Vertical Play",
    side: "Attack",
    map: "Coastline",
    categories: ["Vertical", "Breach"],
    operators: ["Sledge", "Twitch"],
    description: "Twitch drone disables defender gadgets on upper floor while Sledge hammers vertical holes. Creates a three-point entry overwhelming defenders.",
    videoUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    submittedAt: "2026-08-20T18:30:00Z",
    submittedBy: "VerticalG",
    status: "approved",
    type: "strat",
  },
  {
    id: "SUB-006",
    name: "Flank Category",
    side: "Attack",
    map: "Clubhouse",
    categories: [],
    operators: [],
    description: "A category for strategies that exploit flank routes to surprise defenders from unexpected angles.",
    videoUrl: "",
    submittedAt: "2026-08-20T14:10:00Z",
    submittedBy: "StratMaster",
    status: "pending",
    type: "category",
  },
  {
    id: "SUB-007",
    name: "Hard Breach Setup",
    side: "Attack",
    map: "Bank",
    categories: ["Setup", "Breach"],
    operators: ["Thermite", "Thatcher"],
    description: "Standard Thermite + Thatcher combo to clear a reinforced wall. Thatcher removes Bandit's batteries and Bulletproof cameras before Thermite detonates.",
    videoUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    submittedAt: "2026-08-20T11:05:00Z",
    submittedBy: "ClassicStrats",
    status: "rejected",
    type: "strat",
  },
  {
    id: "SUB-008",
    name: "Rappel Category",
    side: "Attack",
    map: "Villa",
    categories: [],
    operators: [],
    description: "Strategies using exterior rappelling as the primary entry vector, creating top-down pressure.",
    videoUrl: "",
    submittedAt: "2026-08-19T16:44:00Z",
    submittedBy: "RoofRunner",
    status: "pending",
    type: "category",
  },
];

export const mockStrats: Strat[] = [
  {
    id: "STR-001",
    name: "Double Breach Garage",
    side: "Attack",
    map: "Clubhouse",
    categories: ["Breach", "Rotate"],
    operators: ["Thermite", "Ash"],
    description: "Simultaneous breach on both garage walls to split defender attention.",
    videoUrl: "https://youtube.com",
    createdAt: "2026-08-15T12:00:00Z",
  },
  {
    id: "STR-002",
    name: "Trophy Room Anchor",
    side: "Defense",
    map: "Clubhouse",
    categories: ["Anchor", "Intel"],
    operators: ["Echo", "Rook"],
    description: "Strong anchor position using Echo drone for area denial.",
    videoUrl: "https://youtube.com",
    createdAt: "2026-08-14T09:30:00Z",
  },
  {
    id: "STR-003",
    name: "Coastline Rappel Rush",
    side: "Attack",
    map: "Coastline",
    categories: ["Rappel", "Vertical"],
    operators: ["Sledge", "Fuze"],
    description: "Coordinated rappel onto terrace while Fuze clusters the floor below.",
    videoUrl: "https://youtube.com",
    createdAt: "2026-08-13T15:20:00Z",
  },
  {
    id: "STR-004",
    name: "Bank Vault Rotation",
    side: "Defense",
    map: "Bank",
    categories: ["Rotate", "Setup"],
    operators: ["Bandit", "Jäger"],
    description: "Pre-built rotation holes from vault to server room for aggressive roaming.",
    videoUrl: "https://youtube.com",
    createdAt: "2026-08-12T11:45:00Z",
  },
  {
    id: "STR-005",
    name: "Consulate Soft Breach Push",
    side: "Attack",
    map: "Consulate",
    categories: ["Breach", "Flank"],
    operators: ["Hibana", "Twitch"],
    description: "Hibana pellets on garage ceiling for vertical control, Twitch kills defender gadgets.",
    videoUrl: "https://youtube.com",
    createdAt: "2026-08-11T08:00:00Z",
  },
];

export const mockCategories: Category[] = [
  { id: "CAT-001", name: "Flank", side: "Attack", description: "Strategies exploiting flank routes", stratCount: 34, createdAt: "2026-07-01T00:00:00Z" },
  { id: "CAT-002", name: "Rotate", side: "Defense", description: "Rotation hole setups and cross-floor plays", stratCount: 28, createdAt: "2026-07-01T00:00:00Z" },
  { id: "CAT-003", name: "Setup", side: "Defense", description: "Round-start gadget placement strategies", stratCount: 52, createdAt: "2026-07-01T00:00:00Z" },
  { id: "CAT-004", name: "Anchor", side: "Defense", description: "Static defensive positions and holds", stratCount: 41, createdAt: "2026-07-01T00:00:00Z" },
  { id: "CAT-005", name: "Roam", side: "Defense", description: "Aggressive roaming routes and spawn peeks", stratCount: 37, createdAt: "2026-07-02T00:00:00Z" },
  { id: "CAT-006", name: "Intel", side: "Attack", description: "Information gathering and drone usage", stratCount: 19, createdAt: "2026-07-02T00:00:00Z" },
  { id: "CAT-007", name: "Breach", side: "Attack", description: "Hard and soft breach strategies", stratCount: 63, createdAt: "2026-07-03T00:00:00Z" },
  { id: "CAT-008", name: "Spawn Peek", side: "Attack", description: "Round-start aggressive peeks", stratCount: 15, createdAt: "2026-07-03T00:00:00Z" },
];

export const recentActivity = [
  { id: "SUB-001", name: "Clubhouse Bar Rotate Smoke", type: "Strat", side: "Attack" as Side, map: "Clubhouse", submittedBy: "SiegeProGamer", submittedAt: "2026-08-21T09:14:00Z", status: "pending" as Status },
  { id: "SUB-002", name: "Bank CEO Anchor Hold", type: "Strat", side: "Defense" as Side, map: "Bank", submittedBy: "DefenseKing99", submittedAt: "2026-08-21T08:47:00Z", status: "pending" as Status },
  { id: "SUB-005", name: "Vertical Play", type: "Strat", side: "Attack" as Side, map: "Coastline", submittedBy: "VerticalG", submittedAt: "2026-08-20T18:30:00Z", status: "approved" as Status },
  { id: "SUB-007", name: "Hard Breach Setup", type: "Strat", side: "Attack" as Side, map: "Bank", submittedBy: "ClassicStrats", submittedAt: "2026-08-20T11:05:00Z", status: "rejected" as Status },
  { id: "SUB-006", name: "Flank Category", type: "Category", side: "Attack" as Side, map: "—", submittedBy: "StratMaster", submittedAt: "2026-08-20T14:10:00Z", status: "pending" as Status },
  { id: "SUB-008", name: "Rappel Category", type: "Category", side: "Attack" as Side, map: "Villa", submittedBy: "RoofRunner", submittedAt: "2026-08-19T16:44:00Z", status: "pending" as Status },
];
