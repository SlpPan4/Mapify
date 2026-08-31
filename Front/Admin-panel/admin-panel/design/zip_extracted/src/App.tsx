import { useState, useCallback } from "react";
import Sidebar from "./components/Sidebar";
import Header from "./components/Header";
import Toast from "./components/Toast";
import type { ToastMessage } from "./components/Toast";
import DashboardPage from "./pages/DashboardPage";
import PendingPage from "./pages/PendingPage";
import StratsPage from "./pages/StratsPage";
import StratDetailPage from "./pages/StratDetailPage";
import CategoriesPage from "./pages/CategoriesPage";
import PlaceholderPage from "./pages/PlaceholderPage";
import { mockSubmissions } from "./data/mockData";
import type { Submission, Strat } from "./data/mockData";

type Page = "dashboard" | "pending" | "strats" | "categories" | "operators" | "maps";

const PAGE_TITLES: Record<Page, string> = {
  dashboard: "Dashboard",
  pending: "Pending Submissions",
  strats: "Strats",
  categories: "Categories",
  operators: "Operators",
  maps: "Maps",
};

export default function App() {
  const [page, setPage] = useState<Page>("dashboard");
  const [viewingStrat, setViewingStrat] = useState<Strat | null>(null);
  const [submissions, setSubmissions] = useState<Submission[]>(mockSubmissions);
  const [toasts, setToasts] = useState<ToastMessage[]>([]);

  const addToast = useCallback((message: string, type: ToastMessage["type"] = "success") => {
    const id = Math.random().toString(36).slice(2);
    setToasts((prev) => [...prev, { id, type, message }]);
  }, []);

  const removeToast = useCallback((id: string) => {
    setToasts((prev) => prev.filter((t) => t.id !== id));
  }, []);

  function handleApprove(id: string) {
    setSubmissions((prev) => prev.map((s) => s.id === id ? { ...s, status: "approved" as const } : s));
    addToast("Submission approved successfully", "success");
  }

  function handleReject(id: string) {
    setSubmissions((prev) => prev.map((s) => s.id === id ? { ...s, status: "rejected" as const } : s));
    addToast("Submission rejected", "error");
  }

  const pendingCount = submissions.filter((s) => s.status === "pending").length;

  function renderPage() {
    switch (page) {
      case "dashboard":
        return <DashboardPage />;
      case "pending":
        return <PendingPage submissions={submissions} onApprove={handleApprove} onReject={handleReject} />;
      case "strats":
        if (viewingStrat) {
          return (
            <StratDetailPage
              strat={viewingStrat}
              onBack={() => setViewingStrat(null)}
              onDelete={() => { setViewingStrat(null); addToast("Strat deleted", "error"); }}
              onSave={(_, data) => setViewingStrat((prev) => prev ? { ...prev, ...data } : prev)}
              onToast={addToast}
            />
          );
        }
        return <StratsPage onToast={addToast} onView={(s) => setViewingStrat(s)} />;
      case "categories":
        return <CategoriesPage onToast={addToast} />;
      case "operators":
        return (
          <PlaceholderPage
            title="Operators Management"
            description="Manage Rainbow Six Siege operators and their metadata. This section is coming soon."
            icon={
              <svg className="w-7 h-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
            }
          />
        );
      case "maps":
        return (
          <PlaceholderPage
            title="Maps Management"
            description="View and manage all available maps in the Mapify database. This section is coming soon."
            icon={
              <svg className="w-7 h-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
            }
          />
        );
    }
  }

  return (
    <div className="min-h-full bg-[#0B0B0C] flex">
      <Sidebar currentPage={page} onNavigate={(p) => { setPage(p); setViewingStrat(null); }} pendingCount={pendingCount} />

      {/* Main content area */}
      <div className="flex-1 flex flex-col min-w-0" style={{ marginLeft: "240px" }}>
        <Header
          title={viewingStrat ? viewingStrat.name : PAGE_TITLES[page]}
          onRefresh={() => addToast("Data refreshed", "info")}
        />
        <main className="flex-1 px-6 py-6">
          {renderPage()}
        </main>
      </div>

      <Toast toasts={toasts} onRemove={removeToast} />
    </div>
  );
}
