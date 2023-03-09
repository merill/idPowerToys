import * as React from "react";
import { useState, useEffect } from 'react';
import {
    makeStyles,
    tokens,
} from "@fluentui/react-components";

import { Providers, ProviderState } from "@microsoft/mgt";
import { Login } from '@microsoft/mgt-react';
import { CaDocGenButton } from '../components/CaDocGenButton';

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

    return (
        <>
            <div className={styles.base}>
                <p>Connect to your tenant and export your conditional access policies as a PowerPoint presentation.</p>

                {isSignedIn &&
                    <CaDocGenButton isManual={false} caPolicyJson={""} />
                }

                {!isSignedIn &&
                    <div>
                        <p>Please sign in to generate documentation.</p>
                        <Login />
                    </div>
                }

            </div>
        </>
    );
};



