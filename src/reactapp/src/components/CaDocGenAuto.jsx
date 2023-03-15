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
import { Accordion, AccordionHeader, AccordionItem, AccordionPanel, Checkbox } from "@fluentui/react-components";

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
                            <Checkbox
        checked={
            isMaskPolicy && isMaskGroup && isMaskUser && isMaskServicePrincipal && isMaskApplication && isMaskTenant && isMaskTermsOfUse && isMaskNamedLocation
            ? true
            : !(isMaskPolicy || isMaskGroup || isMaskUser || isMaskServicePrincipal || isMaskApplication || isMaskTenant || isMaskTermsOfUse || isMaskNamedLocation)
            ? false
            : "mixed"
        }
        onChange={(_ev, data) => {
            setIsMaskPolicy(!!data.checked);
            setIsMaskGroup(!!data.checked);
            setIsMaskUser(!!data.checked);
            setIsMaskServicePrincipal(!!data.checked);
            setIsMaskApplication(!!data.checked);
            setIsMaskTenant(!!data.checked);
            setIsMaskTermsOfUse(!!data.checked);
            setIsMaskNamedLocation(!!data.checked);
        }}
        label="Select all"
      /><br />
                            <Checkbox checked={isMaskPolicy} onChange={() => setIsMaskPolicy((checked) => !checked)} label={"Mask policy names"} /><br />
                            <Checkbox checked={isMaskGroup} onChange={() => setIsMaskGroup((checked) => !checked)} label={"Mask group names"} /><br />
                            <Checkbox checked={isMaskUser} onChange={() => setIsMaskUser((checked) => !checked)} label={"Mask user names"} /><br />
                            <Checkbox checked={isMaskServicePrincipal} onChange={() => setIsMaskServicePrincipal((checked) => !checked)} label={"Mask service principal names"} /><br />
                            <Checkbox checked={isMaskApplication} onChange={() => setIsMaskApplication((checked) => !checked)} label={"Mask applications names"} /><br />
                            <Checkbox checked={isMaskTenant} onChange={() => setIsMaskTenant((checked) => !checked)} label={"Mask tenant names"} /><br />
                            <Checkbox checked={isMaskTermsOfUse} onChange={() => setIsMaskTermsOfUse((checked) => !checked)} label={"Mask terms of use names"} /><br />
                            <Checkbox checked={isMaskNamedLocation} onChange={() => setIsMaskNamedLocation((checked) => !checked)} label={"Mask named locations names"} /><br />
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



