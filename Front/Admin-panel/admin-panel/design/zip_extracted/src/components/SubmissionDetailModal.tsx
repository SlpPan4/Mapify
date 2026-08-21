import { useEffect } from "react";
import type { Submission } from "../data/mockData";
import SideBadge, { StatusBadge } from "./SideBadge";

interface Props {
  submission: Submission | null;
  onClose: () => void;
  onApprove: (id: string) => void;
  onReject: (id: string) => void;
}

function formatDate(iso: string) {
  return new Date(iso).toLocaleString("en-US", { year: "numeric", month: "long", day: "numeric", hour: "2-digit", minute: "2-digit" });
}

export default function SubmissionDetailModal({ submission, onClose, onApprove, onReject }: Props) {
  useEffect(() => {
    const handler = (e: KeyboardEvent) => { if (e.key === "Escape") onClose(); };
    window.addEventListener("keydown", handler);
    return () => window.removeEventListener("keydown", handler);
  }, [onClose]);

  if (!submission) return null;

  return (
    <>
      {/* Backdrop */}
      <div
        className="fixed inset-0 z-[100] bg-black/70 backdrop-blur-[2px]"
        onClick={onClose}
      />

      {/* Slide-over panel */}
      <div className="fixed right-0 top-0 h-full w-full max-w-[520px] bg-[#141416] border-l border-[#2A2A2E] z-[110] animate-slide-right flex flex-col shadow-2xl">
        {/* Header */}
        <div className="px-6 py-4 border-b border-[#2A2A2E] flex items-start justify-between gap-4 shrink-0">
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-2 mb-1">
              <span className="text-[#9CA3AF] text-xs font-mono">{submission.id}</span>
              <StatusBadge status={submission.status} />
            </div>
            <h2 className="text-white font-bold text-lg leading-tight">{submission.name}</h2>
          </div>
          <button
            onClick={onClose}
            className="w-7 h-7 flex items-center justify-center rounded text-[#9CA3AF] hover:text-white hover:bg-[#1C1C1F] transition-colors shrink-0 mt-0.5"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Body */}
        <div className="flex-1 overflow-y-auto px-6 py-5 space-y-6">
          {/* Meta row */}
          <div className="flex items-center gap-3 flex-wrap">
            <SideBadge side={submission.side} />
            <div className="flex items-center gap-1.5">
              <svg className="w-3.5 h-3.5 text-[#9CA3AF]" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
              </svg>
              <span className="text-[#9CA3AF] text-sm">{submission.map}</span>
            </div>
            <div className="flex items-center gap-1.5">
              <svg className="w-3.5 h-3.5 text-[#9CA3AF]" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
              <span className="text-[#9CA3AF] text-sm">{submission.submittedBy}</span>
            </div>
          </div>

          {/* Description */}
          <div>
            <div className="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Description</div>
            <p className="text-white/90 text-sm leading-relaxed bg-[#1C1C1F] border border-[#2A2A2E] rounded p-4">
              {submission.description}
            </p>
          </div>

          {/* Video URL */}
          {submission.videoUrl && (
            <div>
              <div className="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Video Reference</div>
              <div className="bg-[#1C1C1F] border border-[#2A2A2E] rounded p-3 flex items-center gap-3">
                <div className="w-8 h-8 bg-[rgba(220,38,38,0.15)] rounded flex items-center justify-center shrink-0">
                  <svg className="w-4 h-4 text-[#DC2626]" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z" />
                    <path strokeLinecap="round" strokeLinejoin="round" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <a
                  href={submission.videoUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-[#60A5FA] text-sm underline underline-offset-2 hover:text-blue-300 transition-colors truncate"
                >
                  {submission.videoUrl}
                </a>
              </div>
            </div>
          )}

          {/* Categories */}
          {submission.categories.length > 0 && (
            <div>
              <div className="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Categories</div>
              <div className="flex flex-wrap gap-1.5">
                {submission.categories.map((c) => (
                  <span key={c} className="px-2.5 py-1 bg-[#1C1C1F] border border-[#2A2A2E] rounded text-xs text-[#9CA3AF] font-mono">
                    {c}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Operators */}
          {submission.operators.length > 0 && (
            <div>
              <div className="text-[10px] font-mono text-[#9CA3AF] uppercase tracking-widest mb-2">Operators</div>
              <div className="flex flex-wrap gap-1.5">
                {submission.operators.map((op) => (
                  <span key={op} className={`px-2.5 py-1 rounded text-xs font-semibold font-mono border
                    ${submission.side === "Attack"
                      ? "bg-[rgba(220,38,38,0.1)] border-[rgba(220,38,38,0.2)] text-[#EF4444]"
                      : "bg-[rgba(37,99,235,0.1)] border-[rgba(37,99,235,0.2)] text-[#60A5FA]"
                    }`}
                  >
                    {op}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Submitted at */}
          <div className="flex items-center gap-2 pt-1">
            <svg className="w-3.5 h-3.5 text-[#9CA3AF]" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span className="text-[#9CA3AF] text-xs font-mono">Submitted {formatDate(submission.submittedAt)}</span>
          </div>
        </div>

        {/* Footer actions */}
        <div className="px-6 py-4 border-t border-[#2A2A2E] flex items-center gap-3 shrink-0">
          <button
            onClick={() => { onApprove(submission.id); onClose(); }}
            className="flex-1 flex items-center justify-center gap-2 py-2.5 bg-[#DC2626] hover:bg-[#EF4444] text-white font-semibold text-sm rounded transition-colors"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
            </svg>
            Approve
          </button>
          <button
            onClick={() => { onReject(submission.id); onClose(); }}
            className="flex-1 flex items-center justify-center gap-2 py-2.5 bg-[#1C1C1F] hover:bg-[#242428] text-[#EF4444] font-semibold text-sm rounded border border-[#DC2626] transition-colors"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
            Reject
          </button>
          <button className="px-4 py-2.5 bg-[#1C1C1F] hover:bg-[#242428] text-[#9CA3AF] hover:text-white font-medium text-sm rounded border border-[#2A2A2E] transition-colors whitespace-nowrap">
            Edit &amp; Approve
          </button>
        </div>
      </div>
    </>
  );
}
