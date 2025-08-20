import OldNewDataDisplayComponent from "./OldNewDataDisplayComponent.jsx"
import { useState } from "react"

export default function UserOperationComponent(props){
    const [displayData, setDisplayData] = useState(false);

    const styles = {
        backgroundColor: displayData ? "#c44c4cff" : "#6ec44cff"
    }

    return (
        <div className="user-operation-class">
            <span>OPERATION: {props.operation}</span>
            <span>TIME (IN UTC): {props.timestamp}</span>

            <button style={styles} onClick={() => setDisplayData(prev => !prev)}>
                {!displayData ? "Show Data" : "Hide Data"}
            </button>

            {displayData && <OldNewDataDisplayComponent
                oldData={props.dataBefore}
                newData={props.dataAfter}
            />}
        </div>
    )
}