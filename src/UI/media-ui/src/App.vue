<template>
  <AppPreLoader>
    <Upload :show="showUpload"></Upload>
    <v-app>
      <router-view name="appbar"></router-view>
      <v-navigation-drawer clipped v-if="showSidebar" app>
        <router-view name="left"></router-view>
      </v-navigation-drawer>

      <v-main :class="{ fullscreen: isFullscreen }">
        <vue-page-transition>
          <router-view />
        </vue-page-transition>
      </v-main>

      <v-snackbar
        v-for="(snack, i) in snacks"
        :key="'snack' + i"
        :value="snack.show"
        rounded
        app
        bottom
        :color="snack.color"
      >
        <v-icon> {{ snack.icon }}</v-icon>
        {{ snack.text }}

        <template v-slot:action="">
          <v-icon @click="snack.show = false" color="white"> mdi-close </v-icon>
        </template>
      </v-snackbar>
    </v-app>
    <v-dialog v-model="mediaViewerOpen" fullscreen>
      <MediaViewer v-if="mediaViewerOpen"></MediaViewer>
    </v-dialog>

    <edit-face-dialog></edit-face-dialog>
  </AppPreLoader>
</template>

<script>
import AppPreLoader from "./components/AppPreLoader";
import Upload from "./components/Media/Upload";
import MediaViewer from "./components/Media/MediaViewer";
import VuePageTransition from "vue-page-transition";
import Vue from "vue";
import { mediaOperationTypeMap } from "./services/mediaOperationService";
import EditFaceDialog from "./components/Face/EditFaceDialog.vue";

Vue.use(VuePageTransition);

export default {
  name: "App",
  components: { AppPreLoader, Upload, MediaViewer, EditFaceDialog },
  created() {
    const self = this;
    this.$socket.on("mediaOperationCompleted", (data) => {
      self.$store.dispatch("snackbar/mediaOperationCompleted", data);
    });
    this.$socket.on("mediaOperationRequestCompleted", (data) => {
      self.$store.dispatch("snackbar/mediaOperationRequestCompleted", data);

      this.$store.dispatch("media/getFolderTree");

      var opType = mediaOperationTypeMap[data.type];

      this.$magic.snack(
        `${opType.completedText} (${data.successCount} items)`,
        "SUCCESS"
      );
    });
  },
  data: () => ({
    dialog: true,
    sizes: [
      {
        text: "Square XS",
        code: "SQ_XS",
      },
      { text: "Square S", code: "SQ_S" },
      { text: "Small", code: "S" },
      { text: "Medium", code: "M" },
      { text: "Large", code: "L" },
    ],
    showUpload: false,
    mediaViewerOpen: false,
  }),
  computed: {
    mediaActions: function () {
      return [
        { text: "Add to album" },
        { text: "Move" },
        { text: "Edit" },
        { text: "Delete" },
      ];
    },
    navMenuItems: function () {
      return [
        {
          text: "Media",
          icon: "mdi-image",
          route: "Home",
        },
        {
          text: "Face",
          icon: "mdi-face-recognition",
          route: "Faces",
        },
        {
          text: "Persons",
          icon: "mdi-account-details",
          route: "Persons",
        },
        {
          text: "Album",
          icon: "mdi-image-album",
          route: "Albums",
        },
        {
          text: "Map",
          icon: "mdi-map-search-outline",
          route: "Map",
        },
      ];
    },
    editModeText: function () {
      return this.$store.state.media.isEditMode ? "Edit" : "View";
    },
    currentMediaId: function () {
      return this.$store.state.media.currentMediaId;
    },
    snacks: function () {
      return this.$store.state.snackbar.snacks.map((snack) => {
        switch (snack.type) {
          case "INFO":
            snack.icon = "mdi-information-outline";
            snack.color = "blue";
            break;
          case "WARN":
            snack.icon = "mdi-alert-outline";
            snack.color = "yellow";
            break;
          case "ERROR":
            snack.icon = "mdi-nuke";
            snack.color = "red";
            break;
          case "SUCCESS":
            snack.icon = "mdi-check-circle-outline";
            snack.color = "green";
            break;
        }

        return snack;
      });
    },
    mediaCount: function () {
      return this.$store.state.media.list.length;
    },
    selectedCount: function () {
      return this.$store.state.media.selectedIndexes.length;
    },
    loading: function () {
      return this.$store.state.media.listLoading;
    },
    showSidebar: function () {
      if (this.$route.meta.hideSidebar) {
        return false;
      }

      return true;
    },
    isFullscreen: function () {
      if (this.$route.meta.fullscreen) {
        return true;
      } else {
        return false;
      }
    },
  },
  watch: {
    currentMediaId: function (newValue) {
      this.mediaViewerOpen = newValue !== null;
    },
  },
  methods: {
    setSize: function (code) {
      this.$store.dispatch("media/setThumbnailSize", code);
    },
    openUpload: function () {
      this.$store.dispatch("media/toggleUploadDialog", true);
    },
    toggleEditMode: function (value) {
      this.$store.dispatch("media/toggleEditMode", value === "edit");
    },
    selectAll: function () {
      this.$store.dispatch("media/selectAll");
    },
  },
};
</script>
<style >
html {
  overflow-x: hidden !important;
  overflow-y: hidden !important;
}

main {
  height: 90vh;
  overflow-y: hidden;
  overflow-x: hidden;
}

.v-dialog--fullscreen {
  overflow: hidden !important;
  overflow-x: hidden !important;
}

::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track:hover {
  background: #b9dff8;
}
::-webkit-scrollbar-track:active {
  background: #dcdfe0;
}

::-webkit-scrollbar-track {
  background: #e3eaf0;
  border: 0px none #ffffff;
  border-radius: 50px;
}

::-webkit-scrollbar-thumb {
  background: #68bef881;
  border: 0px none #ffffff;
  border-radius: 50px;
}
::-webkit-scrollbar-thumb:hover {
  background: #49a3df;
}
::-webkit-scrollbar-thumb:active {
  background: #3481b4;
}
</style>

