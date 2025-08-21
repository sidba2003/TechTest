import LogComponentDisplay from "./LogComponentDisplay.jsx"
import OperationFilters from './OperationFilters.jsx';
import { useState } from 'react';

export default function LogComponent(props){
    console.log("props are", props);
    const [showCreate, setShowCreate] = useState(true);
    const [showUpdate, setShowUpdate] = useState(true);
    const [showDelete, setShowDelete] = useState(true);

    const userOperationsList = props.operations.filter(item => (
        (showCreate && item.operation === "CREATE") || (showUpdate && item.operation === "UPDATE") || (showDelete && item.operation === "DELETE")
    )).map(item => (
        <LogComponentDisplay {...item} />
    ));

    return (
        <div className="user-operation-class">
            <OperationFilters
                showCreate={showCreate}
                setShowCreate={setShowCreate}

                showUpdate={showUpdate}
                setShowUpdate={setShowUpdate}

                showDelete={showDelete}
                setShowDelete={setShowDelete}
            />
            {userOperationsList}
        </div>
    )
}