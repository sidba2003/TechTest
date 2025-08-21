import { useEffect, useState } from "react"
import LogComponent from "../../misc/LogComponent";

export default function ViewLogsComponent() {
    const [logs, setLogs] = useState([]);

    useEffect(
            () => {
                const fetchLogs = async () => {
                    try {
                        const response = await fetch("/api/user-audits");
                        if (!response.ok) {
                            throw new Error(`Response status: ${response.status}`);
                        }

                        const result = await response.json();
                        setLogs(result);
                        console.log("obtianed data is", result);
                    } catch (error) {
                        console.error(error.message);
                    }
                }

                fetchLogs();
            }, []
    );


    return (
        <div className="user-data-main-class">
            <div className="user-data-header-class">
                Application Logs
            </div>

            <div className="user-operations-class">
                { logs.length > 0 ? <LogComponent operations={logs} /> : <span>No logs have been logged into the app so far!</span> }
            </div>
        </div>
    );
}