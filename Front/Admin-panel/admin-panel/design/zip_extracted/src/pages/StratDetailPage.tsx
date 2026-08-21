import { useState } from "react";
import type { Strat } from "../data/mockData";
import SideBadge from "../components/SideBadge";
import CreateEditModal from "../components/CreateEditModal";

interface Props {
  strat: Strat;
  onBack: () => void;
  onDelete: (id: string) => void;
  onSave: (id: string, data: Partial<Strat>) => void;
  onToast: (msg: string, type?: "success" | "error") => void;
}

function MetaBlock({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div>
      <div
        className="text-[11px] tracking-[0.18em] uppercase mb-2 text-[#9CA3AF]"
        style={{ fontFamily: "'Rajdhani', sans-serif", fontWeight: 600 }}
      >
        {label}
      </div>
      {children}
    </div>
  );
}

function StatPill({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="flex flex-col items-center justify-center bg-[#1C1C1F] border border-[#2A2A2E] rounded px-5 py-3 min-w-[100px] gap-1">
      <span className="text-white font-bold text-xl tabular-nums">{value}</span>
      <span className="text-[#9CA3AF] text-[10px] font-mono tracking-wider uppercase">{label}</span>
    </div>
  );
}

export default function StratDetailPage({ strat: initialStrat, onBack, onDelete, onSave, onToast }: Props) {
  const [strat, setStrat] = useState(initialStrat);
  const [editOpen, setEditOpen] = useState(false);
  const [confirmDelete, setConfirmDelete] = useState(false);

  function handleSave(data: Parameters<typeof onSave>[1]) {
    const updated = { ...strat, ...data };
    setStrat(updated as Strat);
    onSave(strat.id, data);
    onToast("Strat updated successfully", "success");
  }

  function handleDelete() {
    onDelete(strat.id);
    onBack();
  }

  const createdDate = new Date(strat.createdAt).toLocaleDateString("en-US", {
    weekday: "long", year: "numeric", month: "long", day: "numeric",
  });
  const createdTime = new Date(strat.createdAt).toLocaleTimeString("en-US", {
    hour: "2-digit", minute: "2-digit",
  });

  return (
    <div className="animate-fade-in max-w-4xl">
      {/* Breadcrumb + back */}
      <div className="flex items-center gap-2 mb-6">
        <button
          onClick={onBack}
          className="flex items-center gap-1.5 text-[#9CA3AF] hover:text-white text-sm transition-colors group"
        >
          <svg className="w-3.5 h-3.5 transition-transform group-hover:-translate-x-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M15 19l-7-7 7-7" />
          </svg>
          Strats
        </button>
        <span className="text-[#2A2A2E]">/</span>
        <span className="text-white text-sm font-medium truncate">{strat.name}</span>
      </div>

      {/* Hero header */}
      <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden mb-5">
        {/* Red accent bar */}
        <div className="h-[3px] bg-gradient-to-r from-[#DC2626] via-[#DC2626] to-transparent" />

        <div className="px-6 py-5 flex items-start justify-between gap-6">
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-3 mb-2 flex-wrap">
              <span className="text-[#9CA3AF] text-xs font-mono">{strat.id}</span>
              <SideBadge side={strat.side} />
            </div>
            <h1 className="text-white font-bold text-2xl leading-tight mb-3">{strat.name}</h1>
            <div className="flex items-center gap-4 text-sm text-[#9CA3AF] flex-wrap">
              <div className="flex items-center gap-1.5">
                <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path strokeLinecap="round" strokeLinejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                <span>{strat.map}</span>
              </div>
              <div className="flex items-center gap-1.5">
                <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>{createdDate} · {createdTime}</span>
              </div>
            </div>
          </div>

          {/* Action buttons */}
          <div className="flex items-center gap-2 shrink-0">
            <button
              onClick={() => setEditOpen(true)}
              className="flex items-center gap-2 px-3.5 py-2 bg-[#141416] hover:bg-[#242428] text-[#9CA3AF] hover:text-white text-sm font-medium rounded border border-[#2A2A2E] transition-colors"
            >
              <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
              </svg>
              Edit
            </button>
            {confirmDelete ? (
              <div className="flex items-center gap-2 animate-fade-in">
                <span className="text-[#EF4444] text-xs font-medium whitespace-nowrap">Confirm delete?</span>
                <button
                  onClick={handleDelete}
                  className="px-3 py-2 bg-[#DC2626] hover:bg-[#EF4444] text-white text-xs font-semibold rounded transition-colors"
                >
                  Yes, delete
                </button>
                <button
                  onClick={() => setConfirmDelete(false)}
                  className="px-3 py-2 bg-[#141416] text-[#9CA3AF] hover:text-white text-xs rounded border border-[#2A2A2E] transition-colors"
                >
                  Cancel
                </button>
              </div>
            ) : (
              <button
                onClick={() => setConfirmDelete(true)}
                className="flex items-center gap-2 px-3.5 py-2 bg-[#141416] hover:bg-[rgba(220,38,38,0.1)] text-[#DC2626] text-sm font-medium rounded border border-[#2A2A2E] hover:border-[#DC2626] transition-colors"
              >
                <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
                Delete
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Stats row */}
      <div className="flex gap-3 mb-5 flex-wrap">
        <StatPill label="Operators" value={strat.operators.length} />
        <StatPill label="Categories" value={strat.categories.length} />
        <StatPill label="Side" value={strat.side} />
        <StatPill label="Map" value={strat.map} />
      </div>

      {/* Two-column detail grid */}
      <div className="grid grid-cols-[1fr_300px] gap-5">
        {/* Left column */}
        <div className="space-y-5">
          {/* Description */}
          <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
            <MetaBlock label="Description">
              {strat.description ? (
                <p className="text-white/90 text-sm leading-relaxed">{strat.description}</p>
              ) : (
                <p className="text-[#9CA3AF] text-sm italic">No description provided.</p>
              )}
            </MetaBlock>
          </div>

          {/* Video */}
          <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
            <MetaBlock label="Video Reference">
              {strat.videoUrl ? (
                <div className="space-y-3">
                  {/* Fake video thumbnail */}
                  <div className="relative w-full aspect-video bg-[#0B0B0C] border border-[#2A2A2E] rounded overflow-hidden flex items-center justify-center group cursor-pointer">
                    <div className="absolute inset-0 bg-gradient-to-br from-[#1C1C1F] to-[#0B0B0C]" />
                    {/* Grid overlay — tactical map aesthetic */}
                    <div className="absolute inset-0 opacity-[0.04]"
                      style={{
                        backgroundImage: "linear-gradient(#DC2626 1px, transparent 1px), linear-gradient(90deg, #DC2626 1px, transparent 1px)",
                        backgroundSize: "32px 32px",
                      }}
                    />
                    <div className="relative flex flex-col items-center gap-3">
                      <a
                        href={strat.videoUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="w-14 h-14 rounded-full bg-[rgba(220,38,38,0.15)] border border-[rgba(220,38,38,0.3)] flex items-center justify-center hover:bg-[rgba(220,38,38,0.25)] transition-colors group-hover:scale-105 transition-transform"
                        onClick={(e) => e.stopPropagation()}
                      >
                        <svg className="w-6 h-6 text-[#DC2626] ml-0.5" fill="currentColor" viewBox="0 0 24 24">
                          <path d="M8 5v14l11-7z" />
                        </svg>
                      </a>
                      <span className="text-[#9CA3AF] text-xs font-mono">Click to open video</span>
                    </div>
                  </div>
                  <a
                    href={strat.videoUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex items-center gap-2 text-[#60A5FA] text-sm hover:text-blue-300 transition-colors"
                  >
                    <svg className="w-3.5 h-3.5 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                      <path strokeLinecap="round" strokeLinejoin="round" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
                    </svg>
                    <span className="truncate">{strat.videoUrl}</span>
                  </a>
                </div>
              ) : (
                <div className="flex items-center justify-center py-10 bg-[#0B0B0C] border border-[#2A2A2E] rounded text-[#9CA3AF] text-sm gap-2">
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M3 8a2 2 0 012-2h8a2 2 0 012 2v8a2 2 0 01-2 2H5a2 2 0 01-2-2V8z" />
                  </svg>
                  No video attached
                </div>
              )}
            </MetaBlock>
          </div>
        </div>

        {/* Right column */}
        <div className="space-y-5">
          {/* Operators */}
          <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
            <MetaBlock label={`Operators (${strat.operators.length})`}>
              {strat.operators.length > 0 ? (
                <div className="flex flex-col gap-2">
                  {strat.operators.map((op) => (
                    <div
                      key={op}
                      className={`flex items-center gap-3 px-3 py-2.5 rounded border
                        ${strat.side === "Attack"
                          ? "bg-[rgba(220,38,38,0.06)] border-[rgba(220,38,38,0.15)]"
                          : "bg-[rgba(37,99,235,0.06)] border-[rgba(37,99,235,0.15)]"
                        }`}
                    >
                      {/* Operator icon placeholder */}
                      <div className={`w-7 h-7 rounded flex items-center justify-center text-[10px] font-bold shrink-0
                        ${strat.side === "Attack" ? "bg-[rgba(220,38,38,0.15)] text-[#EF4444]" : "bg-[rgba(37,99,235,0.15)] text-[#60A5FA]"}`}>
                        {op.slice(0, 2).toUpperCase()}
                      </div>
                      <span className={`text-sm font-semibold ${strat.side === "Attack" ? "text-[#EF4444]" : "text-[#60A5FA]"}`}>
                        {op}
                      </span>
                      <span className={`ml-auto text-[10px] font-mono ${strat.side === "Attack" ? "text-[rgba(220,38,38,0.6)]" : "text-[rgba(37,99,235,0.6)]"}`}>
                        {strat.side.toUpperCase()}
                      </span>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-[#9CA3AF] text-sm italic">No operators assigned.</p>
              )}
            </MetaBlock>
          </div>

          {/* Categories */}
          <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5">
            <MetaBlock label={`Categories (${strat.categories.length})`}>
              {strat.categories.length > 0 ? (
                <div className="flex flex-wrap gap-2">
                  {strat.categories.map((c) => (
                    <span
                      key={c}
                      className="px-3 py-1.5 bg-[#141416] border border-[#2A2A2E] rounded text-xs text-[#9CA3AF] font-mono hover:border-[#DC2626] hover:text-white transition-colors cursor-default"
                    >
                      {c}
                    </span>
                  ))}
                </div>
              ) : (
                <p className="text-[#9CA3AF] text-sm italic">No categories assigned.</p>
              )}
            </MetaBlock>
          </div>

          {/* Meta info */}
          <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-5 space-y-4">
            <MetaBlock label="Map">
              <div className="flex items-center gap-2">
                <svg className="w-4 h-4 text-[#DC2626]" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path strokeLinecap="round" strokeLinejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                <span className="text-white font-medium text-sm">{strat.map}</span>
              </div>
            </MetaBlock>

            <div className="h-px bg-[#2A2A2E]" />

            <MetaBlock label="Created">
              <div className="space-y-0.5">
                <p className="text-white text-sm">{createdDate}</p>
                <p className="text-[#9CA3AF] text-xs font-mono">{createdTime}</p>
              </div>
            </MetaBlock>

            <div className="h-px bg-[#2A2A2E]" />

            <MetaBlock label="Record ID">
              <span className="text-[#9CA3AF] font-mono text-sm">{strat.id}</span>
            </MetaBlock>
          </div>
        </div>
      </div>

      <CreateEditModal
        open={editOpen}
        mode="edit"
        type="strat"
        initial={{
          id: strat.id,
          name: strat.name,
          side: strat.side,
          map: strat.map,
          categories: strat.categories,
          operators: strat.operators,
          description: strat.description,
          videoUrl: strat.videoUrl,
        }}
        onClose={() => setEditOpen(false)}
        onSave={handleSave}
      />
    </div>
  );
}
