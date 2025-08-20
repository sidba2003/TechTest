export default function UserOperationComponent(props){
    return (
        <div className="user-operation-class">
            <span>{props.operation}</span>
            <span>{props.timestamp}</span>
            <span>{props.dataBefore}</span>
            <span>{props.dataAfter}</span>
        </div>
    )
}