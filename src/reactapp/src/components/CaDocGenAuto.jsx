import * as React from "react";
import { useState, useEffect } from 'react';
import {
    makeStyles,
    tokens,
} from "@fluentui/react-components";

import { Providers, ProviderState } from "@microsoft/mgt";
import { Login } from '@microsoft/mgt-react';
import { CaDocGenButton } from '../components/CaDocGenButton';
import { Alert } from "@fluentui/react-components/unstable";
import { OptionsRegular, } from "@fluentui/react-icons";
import { Accordion, AccordionHeader, AccordionItem, AccordionPanel, Switch } from "@fluentui/react-components";

const useStyles = makeStyles({
    base: {
        display: "flex",
        flexDirection: "column",
        "& > label": {
            display: "block",
            marginBottom: tokens.spacingVerticalMNudge,
        },
    },
    textarea: {
        height: "200px",
    },
    wrapper: {
        marginTop: tokens.spacingVerticalM,
        columnGap: "15px",
        display: "flex",
    },
});

function useIsSignedIn(): [boolean] {
    const [isSignedIn, setIsSignedIn] = useState(false);

    useEffect(() => {
        const updateState = () => {
            const provider = Providers.globalProvider;
            setIsSignedIn(provider && provider.state === ProviderState.SignedIn);
        };

        Providers.onProviderUpdated(updateState);
        updateState();

        return () => {
            Providers.removeProviderUpdatedListener(updateState);
        }
    }, []);

    return [isSignedIn];
}

export const CaDocGenAuto = () => {
    const styles = useStyles();
    const [isSignedIn] = useIsSignedIn();
    const [isMaskPolicy, setIsMaskPolicy] = React.useState(false);
    const [isMaskGroup, setIsMaskGroup] = React.useState(false);
    const [isMaskUser, setIsMaskUser] = React.useState(false);
    const [isMaskServicePrincipal, setIsMaskServicePrincipal] = React.useState(false);
    const [isMaskApplication, setIsMaskApplication] = React.useState(false);
    const [isMaskTenant, setIsMaskTenant] = React.useState(false);
    const [isMaskTermsOfUse, setIsMaskTermsOfUse] = React.useState(false);
    const [isMaskNamedLocation, setIsMaskNamedLocation] = React.useState(false);

    const onChangeMaskPolicy = React.useCallback((ev) => { setIsMaskPolicy(ev.currentTarget.checked); }, [setIsMaskPolicy]);
    const onChangeMaskGroup = React.useCallback((ev) => { setIsMaskGroup(ev.currentTarget.checked); }, [setIsMaskGroup]);
    const onChangeMaskUser = React.useCallback((ev) => { setIsMaskUser(ev.currentTarget.checked); }, [setIsMaskUser]);
    const onChangeMaskServicePrincipal = React.useCallback((ev) => { setIsMaskServicePrincipal(ev.currentTarget.checked); }, [setIsMaskServicePrincipal]);
    const onChangeMaskApplication = React.useCallback((ev) => { setIsMaskApplication(ev.currentTarget.checked); }, [setIsMaskApplication]);
    const onChangeMaskTenant = React.useCallback((ev) => { setIsMaskTenant(ev.currentTarget.checked); }, [setIsMaskTenant]);
    const onChangeMaskTermsOfUse = React.useCallback((ev) => { setIsMaskTermsOfUse(ev.currentTarget.checked); }, [setIsMaskTermsOfUse]);
    const onChangeMaskNamedLocation = React.useCallback((ev) => { setIsMaskNamedLocation(ev.currentTarget.checked); }, [setIsMaskNamedLocation]);    

    return (
        <>
            <div className={styles.base}>
                <p>Dynamically generate a PowerPoint presentation for the conditional access policies in your tenant.</p>
                <Accordion collapsible={true}>
                    <AccordionItem value="1">
                        <AccordionHeader icon={<OptionsRegular />}>
                            Settings
                        </AccordionHeader>
                        <AccordionPanel>
                            <p>Use the option below to remove confidential information from the generated presentation.</p>
                            <Switch onChange={onChangeMaskPolicy} label={"Mask policy names"} /><br />
                            <Switch onChange={onChangeMaskGroup} label={"Mask group names"} /><br />
                            <Switch onChange={onChangeMaskUser} label={"Mask user names"} /><br />
                            <Switch onChange={onChangeMaskServicePrincipal} label={"Mask service principal names"} /><br />
                            <Switch onChange={onChangeMaskApplication} label={"Mask applications names"} /><br />
                            <Switch onChange={onChangeMaskTenant} label={"Mask tenant names"} /><br />
                            <Switch onChange={onChangeMaskTermsOfUse} label={"Mask terms of use names"} /><br />
                            <Switch onChange={onChangeMaskNamedLocation} label={"Mask named locations names"} /><br />
                        </AccordionPanel>
                    </AccordionItem>
                </Accordion>

                {isSignedIn &&
                    <>
                        <CaDocGenButton isMaskPolicy={isMaskPolicy} isMaskGroup={isMaskGroup} isMaskUser={isMaskUser} 
                        isMaskServicePrincipal={isMaskServicePrincipal} isMaskApplication={isMaskApplication} isMaskTenant={isMaskTenant} 
                        isMaskTermsOfUse={isMaskTermsOfUse} isMaskNamedLocation={isMaskNamedLocation} />
                    </>
                }

                {!isSignedIn &&
                    <div>
                        <Alert intent="info" >
                            <p>Please sign in to generate documentation.</p>
                            <p>
                                <Login />
                            </p>
                        </Alert>
                    </div>
                }
            </div>
        </>
    );
};



