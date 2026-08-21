import { useState } from "react";

type Page = "dashboard" | "pending" | "strats" | "categories" | "operators" | "maps";

interface SidebarProps {
  currentPage: Page;
  onNavigate: (page: Page) => void;
  pendingCount: number;
}

const navItems: { id: Page; label: string; icon: React.ReactNode; badge?: boolean }[] = [
  {
    id: "dashboard",
    label: "Dashboard",
    icon: (
      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.8}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
      </svg>
    ),
  },
  {
    id: "pending",
    label: "Pending Submissions",
    icon: (
      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.8}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
      </svg>
    ),
    badge: true,
  },
  {
    id: "strats",
    label: "Strats",
    icon: (
      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.8}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
      </svg>
    ),
  },
  {
    id: "categories",
    label: "Categories",
    icon: (
      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.8}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
      </svg>
    ),
  },
  {
    id: "operators",
    label: "Operators",
    icon: (
      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.8}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
      </svg>
    ),
  },
  {
    id: "maps",
    label: "Maps",
    icon: (
      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.8}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
        <path strokeLinecap="round" strokeLinejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
      </svg>
    ),
  },
];

export default function Sidebar({ currentPage, onNavigate, pendingCount }: SidebarProps) {
  const [query, setQuery] = useState("");

  return (
    <aside className="fixed left-0 top-0 h-full w-[240px] bg-[#141416] border-r border-[#2A2A2E] flex flex-col z-50">
      {/* Logo */}
      <div className="px-5 py-5 border-b border-[#2A2A2E]">
        <div className="flex items-center gap-2.5">
          <div className="w-7 h-7 bg-[#DC2626] rounded flex items-center justify-center">
            <svg className="w-4 h-4 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
            </svg>
          </div>
          <div>
            <div className="text-white font-bold text-[15px] tracking-tight leading-none">Mapify</div>
            <div className="text-[#9CA3AF] text-[11px] tracking-[0.2em] uppercase mt-0.5" style={{ fontFamily: "'Rajdhani', sans-serif", fontWeight: 600 }}>Admin Panel</div>
          </div>
        </div>
      </div>

      {/* Nav */}
      <nav className="flex-1 py-4 overflow-y-auto scrollbar-hide">
        <div className="px-3 mb-2">
          <span className="text-[#9CA3AF] text-[12px] tracking-[0.2em] uppercase px-2" style={{ fontFamily: "'Rajdhani', sans-serif", fontWeight: 600 }}>Navigation</span>
        </div>
        <ul className="space-y-0.5 px-2">
          {navItems.map((item) => {
            const active = currentPage === item.id;
            return (
              <li key={item.id}>
                <button
                  onClick={() => onNavigate(item.id)}
                  className={`w-full flex items-center gap-3 px-3 py-2.5 rounded text-sm font-medium transition-all duration-150 group relative
                    ${active
                      ? "bg-[rgba(220,38,38,0.1)] text-white border-l-2 border-[#DC2626] pl-[10px]"
                      : "text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] border-l-2 border-transparent pl-[10px]"
                    }`}
                >
                  <span className={active ? "text-[#DC2626]" : "text-[#9CA3AF] group-hover:text-white transition-colors"}>
                    {item.icon}
                  </span>
                  <span className="flex-1 text-left">{item.label}</span>
                  {item.badge && pendingCount > 0 && (
                    <span className="bg-[#DC2626] text-white text-[10px] font-bold font-mono px-1.5 py-0.5 rounded-full min-w-[20px] text-center leading-none tabular-nums">
                      {pendingCount}
                    </span>
                  )}
                </button>
              </li>
            );
          })}
        </ul>
      </nav>

      {/* Search */}
      <div className="px-3 py-3 border-t border-[#2A2A2E] relative">
        <svg className="w-3.5 h-3.5 text-[#9CA3AF] absolute left-6 top-1/2 -translate-y-1/2 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
          <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          placeholder="Search…"
          className="w-full bg-[#1C1C1F] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] pl-8 pr-3 py-2 focus:outline-none focus:border-[#DC2626] transition-colors"
        />
        {query && (
          <button onClick={() => setQuery("")} className="absolute right-6 top-1/2 -translate-y-1/2 text-[#9CA3AF] hover:text-white">
            <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        )}
      </div>

      {/* Admin profile */}
      <div className="px-3 py-4 border-t border-[#2A2A2E]">
        <div className="flex items-center gap-3 px-2 py-2.5 rounded hover:bg-[#1C1C1F] transition-colors group cursor-pointer">
          <div className="w-8 h-8 rounded bg-[#DC2626] flex items-center justify-center shrink-0">
            <span className="text-white text-xs font-bold">MX</span>
          </div>
          <div className="flex-1 min-w-0">
            <div className="text-white text-sm font-medium truncate">MapifyAdmin</div>
            <div className="text-[#9CA3AF] text-[11px] truncate">Super Admin</div>
          </div>
          <svg className="w-3.5 h-3.5 text-[#9CA3AF] group-hover:text-white transition-colors shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
        </div>
      </div>
    </aside>
  );
}
