import { useState } from "react";
import type { Submission } from "../data/mockData";
import SideBadge, { StatusBadge } from "../components/SideBadge";
import SubmissionDetailModal from "../components/SubmissionDetailModal";

interface Props {
  submissions: Submission[];
  onApprove: (id: string) => void;
  onReject: (id: string) => void;
}

type Tab = "strat" | "category";

function formatDate(iso: string) {
  return new Date(iso).toLocaleString("en-US", { month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" });
}

export default function PendingPage({ submissions, onApprove, onReject }: Props) {
  const [tab, setTab] = useState<Tab>("strat");
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [sideFilter, setSideFilter] = useState<"all" | "Attack" | "Defense">("all");
  const [search, setSearch] = useState("");
  const [detail, setDetail] = useState<Submission | null>(null);

  const pendingAll = submissions.filter((s) => s.status === "pending");

  const rows = pendingAll.filter((s) => {
    if (s.type !== tab) return false;
    if (sideFilter !== "all" && s.side !== sideFilter) return false;
    if (search && !s.name.toLowerCase().includes(search.toLowerCase())) return false;
    return true;
  });

  const allIds = rows.map((r) => r.id);
  const allChecked = allIds.length > 0 && allIds.every((id) => selected.has(id));

  function toggleAll() {
    if (allChecked) {
      setSelected((prev) => { const n = new Set(prev); allIds.forEach((id) => n.delete(id)); return n; });
    } else {
      setSelected((prev) => new Set([...prev, ...allIds]));
    }
  }

  function toggleRow(id: string) {
    setSelected((prev) => { const n = new Set(prev); n.has(id) ? n.delete(id) : n.add(id); return n; });
  }

  function bulkApprove() {
    selected.forEach((id) => onApprove(id));
    setSelected(new Set());
  }

  function bulkReject() {
    selected.forEach((id) => onReject(id));
    setSelected(new Set());
  }

  const stratPending = pendingAll.filter((s) => s.type === "strat").length;
  const catPending = pendingAll.filter((s) => s.type === "category").length;

  return (
    <div className="animate-fade-in space-y-5">
      {/* Tabs */}
      <div className="flex items-center border-b border-[#2A2A2E] gap-1">
        {([["strat", "Strat Submissions", stratPending], ["category", "Category Submissions", catPending]] as const).map(([t, label, count]) => (
          <button
            key={t}
            onClick={() => { setTab(t); setSelected(new Set()); }}
            className={`flex items-center gap-2 px-4 py-2.5 text-sm font-medium border-b-2 -mb-px transition-colors
              ${tab === t
                ? "border-[#DC2626] text-white"
                : "border-transparent text-[#9CA3AF] hover:text-white"
              }`}
          >
            {label}
            {count > 0 && (
              <span className={`text-[10px] font-bold font-mono px-1.5 py-0.5 rounded-full
                ${tab === t ? "bg-[#DC2626] text-white" : "bg-[#1C1C1F] text-[#9CA3AF] border border-[#2A2A2E]"}`}>
                {count}
              </span>
            )}
          </button>
        ))}
      </div>

      {/* Filters + bulk toolbar */}
      <div className="flex items-center gap-3 flex-wrap">
        {/* Search */}
        <div className="relative flex-1 min-w-[200px] max-w-xs">
          <svg className="w-3.5 h-3.5 text-[#9CA3AF] absolute left-3 top-1/2 -translate-y-1/2 pointer-events-none" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Filter by name…"
            className="w-full bg-[#1C1C1F] border border-[#2A2A2E] rounded text-sm text-white placeholder-[#9CA3AF] pl-8 pr-3 py-2 focus:outline-none focus:border-[#DC2626] transition-colors"
          />
        </div>

        {/* Side filter */}
        <div className="flex items-center gap-1 bg-[#1C1C1F] border border-[#2A2A2E] rounded p-1">
          {(["all", "Attack", "Defense"] as const).map((f) => (
            <button
              key={f}
              onClick={() => setSideFilter(f)}
              className={`px-3 py-1 text-xs font-medium rounded transition-colors
                ${sideFilter === f ? "bg-[#DC2626] text-white" : "text-[#9CA3AF] hover:text-white"}`}
            >
              {f === "all" ? "All Sides" : f}
            </button>
          ))}
        </div>

        <div className="flex-1" />

        {/* Bulk actions (show when items selected) */}
        {selected.size > 0 && (
          <div className="flex items-center gap-2 animate-fade-in">
            <span className="text-[#9CA3AF] text-xs font-mono">{selected.size} selected</span>
            <button onClick={bulkApprove} className="flex items-center gap-1.5 px-3 py-1.5 bg-[#16A34A] hover:bg-green-500 text-white text-xs font-semibold rounded transition-colors">
              <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={3}><path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" /></svg>
              Approve All
            </button>
            <button onClick={bulkReject} className="flex items-center gap-1.5 px-3 py-1.5 bg-[#DC2626] hover:bg-[#EF4444] text-white text-xs font-semibold rounded transition-colors">
              <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={3}><path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" /></svg>
              Reject All
            </button>
            <button onClick={() => setSelected(new Set())} className="text-[#9CA3AF] hover:text-white text-xs transition-colors">Clear</button>
          </div>
        )}
      </div>

      {/* Table */}
      <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded overflow-hidden">
        {rows.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-20 gap-4">
            <div className="w-14 h-14 rounded-full bg-[#141416] border border-[#2A2A2E] flex items-center justify-center">
              <svg className="w-6 h-6 text-[#9CA3AF]" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
              </svg>
            </div>
            <div className="text-center">
              <p className="text-white font-medium text-sm">No pending submissions</p>
              <p className="text-[#9CA3AF] text-xs mt-1">All caught up — nothing awaiting review here.</p>
            </div>
          </div>
        ) : (
          <table className="w-full">
            <thead>
              <tr className="border-b border-[#2A2A2E]">
                <th className="px-4 py-2.5 w-10">
                  <input
                    type="checkbox"
                    checked={allChecked}
                    onChange={toggleAll}
                    className="w-3.5 h-3.5 accent-[#DC2626] cursor-pointer"
                  />
                </th>
                {["ID", "Name", "Side", "Map", "Submitted At", "Status", "Actions"].map((h) => (
                  <th key={h} className="text-left text-[10px] text-[#9CA3AF] font-mono tracking-widest uppercase px-4 py-2.5 whitespace-nowrap">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {rows.map((row, i) => (
                <tr
                  key={row.id}
                  className={`table-row-hover border-b border-[#2A2A2E] last:border-b-0 transition-colors
                    ${selected.has(row.id) ? "bg-[rgba(220,38,38,0.06)]" : i % 2 !== 0 ? "bg-[rgba(255,255,255,0.01)]" : ""}`}
                >
                  <td className="px-4 py-3.5">
                    <input
                      type="checkbox"
                      checked={selected.has(row.id)}
                      onChange={() => toggleRow(row.id)}
                      className="w-3.5 h-3.5 accent-[#DC2626] cursor-pointer"
                    />
                  </td>
                  <td className="px-4 py-3.5">
                    <span className="text-[#9CA3AF] text-xs font-mono">{row.id}</span>
                  </td>
                  <td className="px-4 py-3.5">
                    <span className="text-white text-sm font-medium">{row.name}</span>
                  </td>
                  <td className="px-4 py-3.5">
                    <SideBadge side={row.side} />
                  </td>
                  <td className="px-4 py-3.5">
                    <span className="text-[#9CA3AF] text-sm">{row.map}</span>
                  </td>
                  <td className="px-4 py-3.5">
                    <span className="text-[#9CA3AF] text-xs font-mono whitespace-nowrap">{formatDate(row.submittedAt)}</span>
                  </td>
                  <td className="px-4 py-3.5">
                    <StatusBadge status={row.status} />
                  </td>
                  <td className="px-4 py-3.5">
                    <div className="flex items-center gap-1.5">
                      <button
                        onClick={() => setDetail(row)}
                        className="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#141416] border border-transparent hover:border-[#2A2A2E] transition-all"
                        title="View"
                      >
                        <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                          <path strokeLinecap="round" strokeLinejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                        </svg>
                      </button>
                      <button
                        onClick={() => onApprove(row.id)}
                        className="w-7 h-7 flex items-center justify-center rounded text-[#16A34A] hover:text-white hover:bg-[#16A34A] border border-transparent hover:border-[#16A34A] transition-all"
                        title="Approve"
                      >
                        <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
                        </svg>
                      </button>
                      <button
                        onClick={() => onReject(row.id)}
                        className="w-7 h-7 flex items-center justify-center rounded text-[#DC2626] hover:text-white hover:bg-[#DC2626] border border-transparent hover:border-[#DC2626] transition-all"
                        title="Reject"
                      >
                        <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <SubmissionDetailModal
        submission={detail}
        onClose={() => setDetail(null)}
        onApprove={onApprove}
        onReject={onReject}
      />
    </div>
  );
}
