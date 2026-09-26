package com.project.eclipse.modding;

import android.app.Activity;
import android.app.Fragment;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;

import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.OutputStream;

// A headless Fragment receives the document result without replacing UnityPlayerActivity.
public final class ModZipPicker extends Fragment {
    private static final int REQUEST_ZIP = 27182;
    private static final long MAX_ARCHIVE_BYTES = 512L * 1024L * 1024L;
    private String receiver;
    private boolean launched;

    public static void pick(final Activity activity, final String receiver) {
        activity.runOnUiThread(() -> {
            if (activity.getFragmentManager().findFragmentByTag("eclipse_mod_zip_picker") != null) return;
            ModZipPicker picker = new ModZipPicker();
            picker.receiver = receiver;
            activity.getFragmentManager().beginTransaction()
                .add(picker, "eclipse_mod_zip_picker").commit();
        });
    }

    @Override
    public void onCreate(Bundle state) {
        super.onCreate(state);
        if (state != null) {
            receiver = state.getString("receiver");
            launched = state.getBoolean("launched");
        }
        if (launched) return;
        Intent intent = new Intent(Intent.ACTION_OPEN_DOCUMENT);
        intent.addCategory(Intent.CATEGORY_OPENABLE);
        intent.setType("*/*");
        intent.putExtra(Intent.EXTRA_MIME_TYPES, new String[] {
            "application/zip", "application/x-zip-compressed", "application/octet-stream"
        });
        launched = true;
        startActivityForResult(intent, REQUEST_ZIP);
    }

    @Override
    public void onSaveInstanceState(Bundle state) {
        state.putString("receiver", receiver);
        state.putBoolean("launched", launched);
        super.onSaveInstanceState(state);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode != REQUEST_ZIP) return;
        final Activity activity = getActivity();
        final String target = receiver;
        if (activity != null) activity.getFragmentManager().beginTransaction().remove(this).commit();
        if (resultCode != Activity.RESULT_OK || data == null || data.getData() == null) {
            UnityPlayer.UnitySendMessage(target, "OnModZipPicked", "");
            return;
        }
        final Uri uri = data.getData();
        new Thread(() -> copySelection(activity, target, uri), "Eclipse mod ZIP copy").start();
    }

    private static void copySelection(Activity activity, String receiver, Uri uri) {
        File temporary = null;
        try {
            temporary = File.createTempFile("eclipse-mod-", ".zip", activity.getCacheDir());
            try (InputStream input = activity.getContentResolver().openInputStream(uri);
                 OutputStream output = new FileOutputStream(temporary)) {
                if (input == null) throw new java.io.IOException("The selected file could not be opened.");
                byte[] buffer = new byte[65536];
                long total = 0;
                int count;
                while ((count = input.read(buffer)) != -1) {
                    total += count;
                    if (total > MAX_ARCHIVE_BYTES) throw new java.io.IOException("ZIP exceeds 512 MiB.");
                    output.write(buffer, 0, count);
                }
            }
            UnityPlayer.UnitySendMessage(receiver, "OnModZipPicked", temporary.getAbsolutePath());
        } catch (Exception error) {
            if (temporary != null) temporary.delete();
            UnityPlayer.UnitySendMessage(receiver, "OnModZipPickerError", error.getMessage() == null ?
                "Could not read the selected ZIP." : error.getMessage());
        }
    }
}
