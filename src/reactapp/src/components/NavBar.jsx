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
            </ul>
            <h2>Assessments</h2>
            <ul>
                <li>
                    <Button appearance="primary" onClick={() => navigate('/ZeroTrustHome')} >Zero Trust Assessment</Button>
                </li>
                <li>
                    <Button appearance="primary" onClick={() => navigate('/AzureADHome')}>Azure AD Assessment</Button>
                </li>
            </ul>
        </>
    );
}