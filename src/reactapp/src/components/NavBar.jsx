import React from "react";
import { useNavigate } from "react-router-dom";
import {
    Button,
} from "@fluentui/react-components";

export const NavBar = () => {
    const navigate = useNavigate();
    return (
        <>
            <ul>
                <li>
                    <Button appearance="primary" onClick={() => navigate('/')} >Home</Button>
                </li>
                <li>
                    <Button appearance="primary" onClick={() => navigate('/cadoc/Home')} >Conditional Access Documenter</Button>
                </li>
            </ul>
        </>
    );
}