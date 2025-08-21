import OldNewDataDisplayComponent from "./OldNewDataDisplayComponent.jsx";
import  { useState } from 'react';

export default function LogComponentDisplay(props) {
    const [displayData, setDisplayData] = useState(false);
    
    const styles = {
        backgroundColor: displayData ? "#c44c4cff" : "#6ec44cff"
    }

    return (
        <div className="operation-class">
            <span><strong>OPERATION:</strong> {props.operation}</span>
            <span><strong>TIME (IN UTC):</strong> {props.timestamp}</span>

            <button style={styles} onClick={() => setDisplayData(prev => !prev)} className="user-operation-button-class">
                {!displayData ? "Click Here to See Additional Data" : "Hide Data"}
            </button>

            {displayData && <OldNewDataDisplayComponent
                oldData={props.dataBefore}
                newData={props.dataAfter}
            />}
        </div>
    )
}