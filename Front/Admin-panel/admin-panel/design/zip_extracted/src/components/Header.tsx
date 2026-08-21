import { useState } from "react";

interface HeaderProps {
  title: string;
  onRefresh: () => void;
}

export default function Header({ title, onRefresh }: HeaderProps) {
  const [notifOpen, setNotifOpen] = useState(false);

  return (
    <header className="h-14 border-b border-[#2A2A2E] bg-[#141416] flex items-center px-6 gap-4 sticky top-0 z-40">
      <h1 className="text-white font-semibold text-[15px] tracking-tight shrink-0 mr-2">{title}</h1>

      <div className="flex items-center gap-2 ml-auto">
        {/* Refresh */}
        <button
          onClick={onRefresh}
          className="w-8 h-8 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-all border border-transparent hover:border-[#2A2A2E]"
          title="Refresh"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
        </button>

        {/* Notification bell */}
        <div className="relative">
          <button
            onClick={() => setNotifOpen(!notifOpen)}
            className="w-8 h-8 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-all border border-transparent hover:border-[#2A2A2E] relative"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
            </svg>
            <span className="absolute top-1.5 right-1.5 w-1.5 h-1.5 bg-[#DC2626] rounded-full ring-1 ring-[#141416]" />
          </button>

          {notifOpen && (
            <>
              <div className="fixed inset-0 z-40" onClick={() => setNotifOpen(false)} />
              <div className="absolute right-0 top-10 w-72 bg-[#1C1C1F] border border-[#2A2A2E] rounded shadow-2xl z-50 animate-fade-in">
                <div className="px-4 py-3 border-b border-[#2A2A2E]">
                  <span className="text-sm font-semibold text-white">Notifications</span>
                </div>
                {[
                  { text: "New strat submission from SiegeProGamer", time: "9 min ago" },
                  { text: "DefenseKing99 submitted Bank CEO Anchor Hold", time: "32 min ago" },
                  { text: "3 submissions pending review", time: "1 hr ago" },
                ].map((n, i) => (
                  <div key={i} className="px-4 py-3 border-b border-[#2A2A2E] last:border-b-0 hover:bg-[#141416] transition-colors cursor-pointer">
                    <p className="text-sm text-white leading-snug">{n.text}</p>
                    <p className="text-[11px] text-[#9CA3AF] mt-1 font-mono">{n.time}</p>
                  </div>
                ))}
              </div>
            </>
          )}
        </div>

        {/* Divider */}
        <div className="w-px h-5 bg-[#2A2A2E]" />

        {/* Avatar */}
        <div className="w-7 h-7 rounded bg-[#DC2626] flex items-center justify-center cursor-pointer hover:bg-[#EF4444] transition-colors">
          <span className="text-white text-[11px] font-bold">MX</span>
        </div>
      </div>
    </header>
  );
}
