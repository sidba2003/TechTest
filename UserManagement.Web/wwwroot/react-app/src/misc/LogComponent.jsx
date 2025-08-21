import LogComponentDisplay from "./LogComponentDisplay.jsx";
import OperationFilters from "./OperationFilters.jsx";
import { useEffect, useMemo, useState } from "react";

export default function LogComponent({ operations = [] }) {
    const [showCreate, setShowCreate] = useState(true);
    const [showUpdate, setShowUpdate] = useState(true);
    const [showDelete, setShowDelete] = useState(true);

    // pagination state
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);

    // filter first
    const filtered = useMemo(() => {
        return operations.filter(
        (item) =>
            (showCreate && item.operation === "CREATE") ||
            (showUpdate && item.operation === "UPDATE") ||
            (showDelete && item.operation === "DELETE")
        );
    }, [operations, showCreate, showUpdate, showDelete]);

    // reset page when filters/data change
    useEffect(() => {
        setPage(1);
    }, [showCreate, showUpdate, showDelete, operations]);

    const totalItems = filtered.length;
    const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));

    // clamp page if list shrinks or pageSize changes
    useEffect(() => {
        if (page > totalPages) setPage(totalPages);
    }, [totalPages, page]);

    // page slice (indexing unchanged)
    const startIndex = (page - 1) * pageSize;
    const endIndex = Math.min(startIndex + pageSize, totalItems);
    const pageItems = useMemo(
        () => filtered.slice(startIndex, endIndex),
        [filtered, startIndex, endIndex]
    );

    return (
        <div className="log-view">
        <OperationFilters
            showCreate={showCreate}
            setShowCreate={setShowCreate}
            showUpdate={showUpdate}
            setShowUpdate={setShowUpdate}
            showDelete={showDelete}
            setShowDelete={setShowDelete}
        />

        <div className="results-summary" aria-live="polite">
            {totalItems === 0
            ? "No operations to display"
            : `${startIndex + 1}–${endIndex} of ${totalItems}`}
        </div>

        <ul className="log-list">
            {pageItems.map((item, idx) => (
            <li
                className="log-list__item"
                key={
                item.id ??
                item._id ??
                `${item.operation}-${item.timestamp ?? ""}-${startIndex + idx}`
                }
            >
                <LogComponentDisplay {...item} />
            </li>
            ))}
        </ul>

        <Pagination
            page={page}
            setPage={setPage}
            totalPages={totalPages}
            pageSize={pageSize}
            setPageSize={setPageSize}
        />
        </div>
    );
}

function Pagination({ page, setPage, totalPages, pageSize, setPageSize }) {
    const canPrev = page > 1;
    const canNext = page < totalPages;

    const go = (p) => setPage(Math.min(Math.max(1, p), totalPages));

    return (
        <nav className="pagination" aria-label="Pagination">
        <button onClick={() => go(1)} disabled={!canPrev} className="pagination__btn">
            First
        </button>
        <button
            onClick={() => go(page - 1)}
            disabled={!canPrev}
            className="pagination__btn"
        >
            Prev
        </button>

        <span className="pagination__page" aria-live="polite">
            Page {page} of {totalPages}
        </span>

        <button
            onClick={() => go(page + 1)}
            disabled={!canNext}
            className="pagination__btn"
        >
            Next
        </button>
        <button
            onClick={() => go(totalPages)}
            disabled={!canNext}
            className="pagination__btn"
        >
            Last
        </button>

        <label className="page-size">
            Page size
            <select
            value={pageSize}
            onChange={(e) => setPageSize(Number(e.target.value))}
            className="page-size__select"
            >
            {[5, 10, 20, 50].map((s) => (
                <option key={s} value={s}>
                {s}
                </option>
            ))}
            </select>
        </label>
        </nav>
    );
}
